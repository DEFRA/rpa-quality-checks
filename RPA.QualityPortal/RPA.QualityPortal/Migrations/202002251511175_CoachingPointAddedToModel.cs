namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class CoachingPointAddedToModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccount", "CoachingPoint", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccount", "CoachingPoint");
        }
    }
}
