namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class UpdatedFailReasonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FailReason", "CheckTypeId", c => c.Int());
            CreateIndex("dbo.FailReason", "CheckTypeId");
            AddForeignKey("dbo.FailReason", "CheckTypeId", "dbo.CheckType", "CheckTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FailReason", "CheckTypeId", "dbo.CheckType");
            DropIndex("dbo.FailReason", new[] { "CheckTypeId" });
            DropColumn("dbo.FailReason", "CheckTypeId");
        }
    }
}
