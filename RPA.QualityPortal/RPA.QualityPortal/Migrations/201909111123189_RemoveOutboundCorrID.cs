namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class RemoveOutboundCorrID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OutboundCorrespondence", "OutBoundCorresondenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OutboundCorrespondence", "OutBoundCorresondenceId", c => c.Int(nullable: false));
        }
    }
}
