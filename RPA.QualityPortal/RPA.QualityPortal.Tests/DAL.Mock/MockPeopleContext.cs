using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Tests.Data.Mock;

namespace RPA.QualityPortal.Tests.DAL.Mock
{
    public class MockPeopleContext : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly PeopleContext db;

        public ContextHandle<IPeopleContext> MockContext { get; }

        public PeopleContext Db => db;

        public DbSetHandle<Person> MockPeople { get; private set; }

        public MockPeopleContext(bool setMocks = true)
        {
            connection = TestDb.OpenConnection();
            TestDb.AttachSchema(connection, "OLA");
            var options = new DbContextOptionsBuilder<PeopleContext>().UseSqlite(connection).Options;
            db = new PeopleContext(options);
            db.Database.EnsureCreated();
            MockContext = new ContextHandle<IPeopleContext>(db, () => 0);
            MockPeople = new DbSetHandle<Person>(db.People);

            if (setMocks)
            {
                SetMocks();
            }
        }

        public void SetMocks()
        {
            TestDb.Seed(db, LocationData.Data());
            TestDb.Seed(db, ManagerData.Data());
            TestDb.Seed(db, PeopleData.Data());
            db.SaveChanges();
            db.ChangeTracker.Clear();
        }

        public void Dispose()
        {
            db.Dispose();
            connection.Dispose();
        }
    }

    public class DbSetHandle<T> where T : class
    {
        public DbSetHandle(DbSet<T> set) { Object = set; }

        public DbSet<T> Object { get; }
    }
}
