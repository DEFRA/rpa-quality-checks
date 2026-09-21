namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeCheckResultFailReason : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" }, "dbo.CheckResult");
            DropForeignKey("dbo.CheckResultFailReason", "FailReasonId", "dbo.FailReason");
            DropIndex("dbo.CheckResultFailReason", new[] { "FailReasonId" });
            DropIndex("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" });
            DropTable("dbo.CheckResultFailReason");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CheckResultFailReason",
                c => new
                    {
                        CheckResultId = c.Int(nullable: false),
                        FailReasonId = c.Int(nullable: false),
                        CheckResult_CheckId = c.Int(),
                        CheckResult_ResultId = c.Int(),
                    })
                .PrimaryKey(t => new { t.CheckResultId, t.FailReasonId });
            
            CreateIndex("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" });
            CreateIndex("dbo.CheckResultFailReason", "FailReasonId");
            AddForeignKey("dbo.CheckResultFailReason", "FailReasonId", "dbo.FailReason", "FailReasonId", cascadeDelete: true);
            AddForeignKey("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" }, "dbo.CheckResult", new[] { "CheckId", "ResultId" });
        }
    }
}
