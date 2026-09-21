namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class controlAdded : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("dbo.Control");
            //AlterColumn("dbo.Control", "ControlId", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.Control", "ControlId");

            CreateTable(
                "dbo.Control",
                c => new
                {
                    ControlId = c.Int(nullable: false),
                    Property = c.String(),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ControlId);
        }

        public override void Down()
        {
            //DropPrimaryKey("dbo.Control");
            //AlterColumn("dbo.Control", "ControlId", c => c.Guid(nullable: false));
            //AddPrimaryKey("dbo.Control", "ControlId");
        }
    }
}
