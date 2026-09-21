namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class EmailNotificationsDefaultToTrue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean(nullable: false));
        }
    }
}
