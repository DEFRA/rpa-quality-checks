namespace RPA.QualityPortal.Migrations
{
    using RPA.QualityPortal.Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal sealed class Configuration : DbMigrationsConfiguration<DAL.QualityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            ContextKey = "RPA.QualityPortal.DAL.QualityContext";
        }

        public Configuration(string connectionString) : this()
        {
            TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(connectionString, "System.Data.SqlClient");
        }

        protected override void Seed(DAL.QualityContext context)
        {

        }
    }
}
