namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class FailReasonNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReasonId" });
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int());
            CreateIndex("dbo.OutboundCorrespondence", "FailReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason", "FailReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason");
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReasonId" });
            AlterColumn("dbo.OutboundCorrespondence", "FailReasonId", c => c.Int(nullable: false));
            CreateIndex("dbo.OutboundCorrespondence", "FailReasonId");
            AddForeignKey("dbo.OutboundCorrespondence", "FailReasonId", "dbo.FailReason", "FailReasonId", cascadeDelete: true);
        }
    }
}
