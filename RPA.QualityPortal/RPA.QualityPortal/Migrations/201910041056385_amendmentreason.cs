namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class amendmentreason : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReasonId" });
            AddColumn("dbo.Audit", "Comments", c => c.String());
            AddColumn("dbo.Audit", "AmendmentReason_AmendmentReasonId", c => c.Int());
            AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonId", c => c.Int(nullable: false));
            AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments", c => c.String());
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int());
            CreateIndex("dbo.Audit", "AmendmentReason_AmendmentReasonId");
            CreateIndex("dbo.OutboundCorrespondence", "FailReasonId");
            CreateIndex("dbo.OutboundCorrespondence", "AmendmentReasonId");
            AddForeignKey("dbo.Audit", "AmendmentReason_AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId", cascadeDelete: true);
            AddForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason", "FailReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason");
            DropForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason");
            DropForeignKey("dbo.Audit", "AmendmentReason_AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "AmendmentReasonId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReasonId" });
            DropIndex("dbo.Audit", new[] { "AmendmentReason_AmendmentReasonId" });
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int(nullable: false));
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments");
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonId");
            DropColumn("dbo.Audit", "AmendmentReason_AmendmentReasonId");
            DropColumn("dbo.Audit", "Comments");
            CreateIndex("dbo.OutboundCorrespondence", "FailReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason", "FailReasonId", cascadeDelete: true);
        }
    }
}
