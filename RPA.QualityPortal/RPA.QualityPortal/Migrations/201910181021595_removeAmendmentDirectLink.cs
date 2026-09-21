namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeAmendmentDirectLink : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "AmendmentReasonId" });
            DropPrimaryKey("dbo.CheckResult");
            //CreateTable(
            //    "dbo.CheckAmendmentReason",
            //    c => new
            //        {
            //            CheckAmendmentReasonId = c.Int(nullable: false, identity: true),
            //            CheckId = c.Int(nullable: false),
            //            AmendmentReasonId = c.Int(nullable: false),
            //            AmendmentReasonComments = c.String(),
            //        })
            //    .PrimaryKey(t => t.CheckAmendmentReasonId)
            //    .ForeignKey("dbo.AmendmentReason", t => t.AmendmentReasonId, cascadeDelete: true)
            //    .ForeignKey("dbo.Check", t => t.CheckId, cascadeDelete: true)
            //    .Index(t => t.CheckId)
            //    .Index(t => t.AmendmentReasonId);

            AddColumn("dbo.CheckResult", "CheckResultId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CheckResult", "CheckResultId");
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonId");
            //DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments", c => c.String());
            AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonId", c => c.Int());
            DropForeignKey("dbo.CheckAmendmentReason", "CheckId", "dbo.Check");
            DropForeignKey("dbo.CheckAmendmentReason", "AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.CheckAmendmentReason", new[] { "AmendmentReasonId" });
            DropIndex("dbo.CheckAmendmentReason", new[] { "CheckId" });
            DropPrimaryKey("dbo.CheckResult");
            DropColumn("dbo.CheckResult", "CheckResultId");
            DropTable("dbo.CheckAmendmentReason");
            AddPrimaryKey("dbo.CheckResult", new[] { "CheckId", "ResultId" });
            CreateIndex("dbo.OutboundCorrespondence", "AmendmentReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
        }
    }
}
