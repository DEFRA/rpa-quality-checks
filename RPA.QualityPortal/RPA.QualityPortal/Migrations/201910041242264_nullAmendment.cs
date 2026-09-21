namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class nullAmendment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "AmendmentReasonId" });
            AlterColumn("dbo.OutboundCorrespondence", "AmendmentReasonId", c => c.Int());
            CreateIndex("dbo.OutboundCorrespondence", "AmendmentReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "AmendmentReasonId" });
            AlterColumn("dbo.OutboundCorrespondence", "AmendmentReasonId", c => c.Int(nullable: false));
            CreateIndex("dbo.OutboundCorrespondence", "AmendmentReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "AmendmentReasonId", "dbo.AmendmentReason", "AmendmentReasonId", cascadeDelete: true);
        }
    }
}
