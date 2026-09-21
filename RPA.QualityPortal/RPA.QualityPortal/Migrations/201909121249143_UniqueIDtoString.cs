namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class UniqueIDtoString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OutboundCorrespondence", "UniqueId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OutboundCorrespondence", "UniqueId", c => c.Int(nullable: false));
        }
    }
}
