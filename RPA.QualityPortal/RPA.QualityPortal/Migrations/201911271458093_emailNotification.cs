namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class emailNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Check", "EmailNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Check", "EmailNotifications");
        }
    }
}
