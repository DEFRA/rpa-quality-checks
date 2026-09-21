namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class merge : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Audit", "Comments", c => c.String());
            //AddColumn("dbo.Audit", "AmendmentReason_AmendmentReasonId", c => c.Int());
            //AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonId", c => c.Int());
            //AddColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments", c => c.String());
            //CreateIndex("dbo.Audit", "AmendmentReason_AmendmentReasonId");
            //CreateIndex("dbo.OutboundCorrespondence", "AmendmentReasonId");
            //AddForeignKey("dbo.Audit", "AmendmentReason_AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
            //AddForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason");
            DropForeignKey("dbo.Audit", "AmendmentReason_AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "AmendmentReasonId" });
            DropIndex("dbo.Audit", new[] { "AmendmentReason_AmendmentReasonId" });
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonComments");
            DropColumn("dbo.OutboundCorrespondence", "AmendmentReasonId");
            DropColumn("dbo.Audit", "AmendmentReason_AmendmentReasonId");
            DropColumn("dbo.Audit", "Comments");
        }
    }
}
