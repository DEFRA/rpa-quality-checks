namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class reCheckAmendment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckAmendmentReason",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        AmendmentReasonId = c.Int(nullable: false),
                        AmendmentReasonComments = c.String(),
                    })
                .PrimaryKey(t => new { t.CheckId, t.AmendmentReasonId })
                .ForeignKey("dbo.AmendmentReason", t => t.AmendmentReasonId, cascadeDelete: true)
                .ForeignKey("dbo.Check", t => t.CheckId, cascadeDelete: true)
                .Index(t => t.CheckId)
                .Index(t => t.AmendmentReasonId);
            
            AddColumn("dbo.OutboundCorrespondenceReCheck", "AmendmentReasonId", c => c.Int());
            CreateIndex("dbo.OutboundCorrespondenceReCheck", "AmendmentReasonId");
            AddForeignKey("dbo.OutboundCorrespondenceReCheck", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments", c => c.String());
            DropForeignKey("dbo.OutboundCorrespondenceReCheck", "AmendmentReasonId", "dbo.AmendmentReason");
            DropForeignKey("dbo.CheckAmendmentReason", "CheckId", "dbo.Check");
            DropForeignKey("dbo.CheckAmendmentReason", "AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondenceReCheck", new[] { "AmendmentReasonId" });
            DropIndex("dbo.CheckAmendmentReason", new[] { "AmendmentReasonId" });
            DropIndex("dbo.CheckAmendmentReason", new[] { "CheckId" });
            DropColumn("dbo.OutboundCorrespondenceReCheck", "AmendmentReasonId");
            DropTable("dbo.CheckAmendmentReason");
        }
    }
}
