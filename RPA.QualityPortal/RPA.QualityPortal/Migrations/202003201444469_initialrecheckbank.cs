namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class initialrecheckbank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccount", "InitialReCheckDecision", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccount", "InitialReCheckDecision");
        }
    }
}
