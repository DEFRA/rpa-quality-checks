namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class initialReCheckDecisionAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OutboundCorrespondence", "InitialReCheckDecision", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OutboundCorrespondence", "InitialReCheckDecision");
        }
    }
}
