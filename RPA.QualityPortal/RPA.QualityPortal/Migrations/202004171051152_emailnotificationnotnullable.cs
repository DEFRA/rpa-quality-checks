namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class emailnotificationnotnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Check", "EmailNotifications", c => c.Boolean());
        }
    }
}
