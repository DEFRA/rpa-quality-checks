namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeGuidfromAudit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Audit", "Check_CheckId", "dbo.Check");
            DropIndex("dbo.Audit", new[] { "Check_CheckId" });
            DropColumn("dbo.Audit", "CheckId");
            RenameColumn(table: "dbo.Audit", name: "Check_CheckId", newName: "CheckId");
            AlterColumn("dbo.Audit", "CheckId", c => c.Int(nullable: false));
            AlterColumn("dbo.Audit", "CheckId", c => c.Int(nullable: false));
            CreateIndex("dbo.Audit", "CheckId");
            AddForeignKey("dbo.Audit", "CheckId", "dbo.Check", "CheckId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Audit", "CheckId", "dbo.Check");
            DropIndex("dbo.Audit", new[] { "CheckId" });
            AlterColumn("dbo.Audit", "CheckId", c => c.Int());
            AlterColumn("dbo.Audit", "CheckId", c => c.Guid(nullable: false));
            RenameColumn(table: "dbo.Audit", name: "CheckId", newName: "Check_CheckId");
            AddColumn("dbo.Audit", "CheckId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Audit", "Check_CheckId");
            AddForeignKey("dbo.Audit", "Check_CheckId", "dbo.Check", "CheckId");
        }
    }
}
