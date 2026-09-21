namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class setEmailNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean(nullable: false));
        }
    }
}
