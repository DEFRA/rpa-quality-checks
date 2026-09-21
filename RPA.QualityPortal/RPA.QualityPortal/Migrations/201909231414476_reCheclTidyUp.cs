namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class reCheclTidyUp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId", "dbo.OutboundCorrespondence");
            DropIndex("dbo.OutboundCorrespondenceReCheck", new[] { "OutboundCorrespondence_CheckId" });
            AddColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondenceId", c => c.Int(nullable: false));
            AddColumn("dbo.OutboundCorrespondenceReCheck", "ReCheckCompletedBy", c => c.String(nullable: false));
            DropColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId");
            DropColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondenceReCheckId");
            DropColumn("dbo.OutboundCorrespondenceReCheck", "OutBoundCorresondenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OutboundCorrespondenceReCheck", "OutBoundCorresondenceId", c => c.Int(nullable: false));
            AddColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondenceReCheckId", c => c.Int(nullable: false));
            AddColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId", c => c.Int());
            DropColumn("dbo.OutboundCorrespondenceReCheck", "ReCheckCompletedBy");
            DropColumn("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondenceId");
            CreateIndex("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId");
            AddForeignKey("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId", "dbo.OutboundCorrespondence", "CheckId");
        }
    }
}
