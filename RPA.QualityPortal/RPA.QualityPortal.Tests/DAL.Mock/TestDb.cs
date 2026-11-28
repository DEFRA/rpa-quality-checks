using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using RPA.QualityPortal.DAL;
using System.Linq.Expressions;

namespace RPA.QualityPortal.Tests.DAL.Mock
{
    // Wraps a real EF Core context so existing tests can keep calling Object / Verify(SaveChanges) without Moq DbSet mocks.
    public class ContextHandle<TInterface> where TInterface : class
    {
        private readonly Func<int> saveCount;

        public ContextHandle(TInterface context, Func<int> saveCount)
        {
            Object = context;
            this.saveCount = saveCount;
        }

        public TInterface Object { get; }

        public void Verify(Expression<Func<TInterface, int>> expression, Times times)
        {
            var count = saveCount();
            if (!times.Validate(count))
            {
                NUnit.Framework.Assert.Fail($"SaveChanges was called {count} time(s); expected {times}.");
            }
        }
    }

    public class CountingQualityContext : QualityContext
    {
        public CountingQualityContext(DbContextOptions<QualityContext> options) : base(options) { }

        public int SaveCount { get; set; }

        public bool EmulateIdentity { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SaveCount++;

            // Legacy tests post entities carrying explicit ids; EF6 ignored them for identity columns, so emulate that.
            foreach (var entry in ChangeTracker.Entries().Where(e => EmulateIdentity && e.State == EntityState.Added))
            {
                var key = entry.Metadata.FindPrimaryKey();
                if (key.Properties.Count == 1 && key.Properties[0].ClrType == typeof(int) && key.Properties[0].ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd)
                {
                    entry.Property(key.Properties[0].Name).IsTemporary = true;
                }
            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
    }

    public static class TestDb
    {
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<DbContext, Dictionary<Type, HashSet<string>>> SeenKeys = new();

        // Shared open in-memory connection: the database lives as long as the connection. FK enforcement is off
        // because the legacy mock data contains rows that do not reference each other consistently.
        public static SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
            connection.Open();
            return connection;
        }

        public static void AttachSchema(SqliteConnection connection, string schema)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ATTACH DATABASE ':memory:' AS {schema}";
            command.ExecuteNonQuery();
        }

        // Copies scalar values only (navigation graphs from the legacy mock data are rebuilt by EF fix-up from the keys).
        public static void Seed<T>(DbContext db, IEnumerable<T> items) where T : class
        {
            var entityType = db.Model.FindEntityType(typeof(T));
            var key = entityType.FindPrimaryKey();
            // TPT subtypes share the root table's key space, so duplicate detection is per root type.
            var seenByRoot = SeenKeys.GetOrCreateValue(db);
            var root = entityType.GetRootType().ClrType;
            if (!seenByRoot.TryGetValue(root, out var seen))
            {
                seenByRoot[root] = seen = new HashSet<string>();
            }

            foreach (var item in items)
            {
                var copy = (T)Activator.CreateInstance(typeof(T));
                db.Entry(copy).CurrentValues.SetValues(item);

                var values = key.Properties.Select(p => p.PropertyInfo.GetValue(copy)).ToArray();
                var isGeneratedKey = key.Properties.Count == 1 && values[0] is int id && id == 0;
                if (!isGeneratedKey && !seen.Add(string.Join("|", values)))
                {
                    if (key.Properties.Count == 1 && values[0] is int)
                    {
                        var next = seen.Select(s => long.TryParse(s, out var n) ? n : 0).Max() + 1;
                        key.Properties[0].PropertyInfo.SetValue(copy, (int)next);
                        seen.Add(next.ToString());
                    }
                    else
                    {
                        continue;
                    }
                }

                db.Add(copy);
            }
        }
    }
}
