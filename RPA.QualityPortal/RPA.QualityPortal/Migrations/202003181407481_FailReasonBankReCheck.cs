namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class FailReasonBankReCheck : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Check", "ReCheckCoachingPoint", c => c.Boolean());
            AddColumn("dbo.Check", "FailReasonId1", c => c.Int());
            CreateIndex("dbo.Check", "FailReasonId1");
            AddForeignKey("dbo.Check", "FailReasonId1", "dbo.FailReason", "FailReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Check", "FailReasonId1", "dbo.FailReason");
            DropIndex("dbo.Check", new[] { "FailReasonId1" });
            DropColumn("dbo.Check", "FailReasonId1");
            DropColumn("dbo.Check", "ReCheckCoachingPoint");
        }
    }
}
