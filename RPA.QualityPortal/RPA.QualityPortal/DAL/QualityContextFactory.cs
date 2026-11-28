using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.DAL
{
    // Used only by `dotnet ef` so migrations can be created and scripted without starting the web host
    // (which requires the full identity configuration). Set ConnectionStrings__QualityContext to target a real database.
    [ExcludeFromCodeCoverage]
    public class QualityContextFactory : IDesignTimeDbContextFactory<QualityContext>
    {
        public QualityContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__QualityContext")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=QualityPortal;Trusted_Connection=True;";

            var options = new DbContextOptionsBuilder<QualityContext>()
                .UseSqlServer(connectionString, sql => sql.CommandTimeout(1000))
                .Options;

            return new QualityContext(options);
        }
    }
}
