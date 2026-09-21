namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class checkAmendmentReason : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CheckAmendmentReason");
            AddColumn("dbo.CheckAmendmentReason", "CheckAmendmentReasonId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.CheckAmendmentReason", "CheckAmendmentReasonId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CheckAmendmentReason");
            DropColumn("dbo.CheckAmendmentReason", "CheckAmendmentReasonId");
            AddPrimaryKey("dbo.CheckAmendmentReason", new[] { "CheckId", "AmendmentReasonId" });
        }
    }
}
