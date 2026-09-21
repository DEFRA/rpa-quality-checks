namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class BankAccountTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BACheckType",
                c => new
                    {
                        BACheckTypeId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BACheckTypeId);
            
            CreateTable(
                "dbo.BankAccount",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        SBI = c.String(),
                        FRN = c.Int(nullable: false),
                        BACheckTypeId = c.Int(nullable: false),
                        BusinessName = c.String(nullable: false),
                        ManagerName = c.String(nullable: false),
                        DateQcCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Check", t => t.CheckId)
                .ForeignKey("dbo.BACheckType", t => t.BACheckTypeId, cascadeDelete: true)
                .Index(t => t.CheckId)
                .Index(t => t.BACheckTypeId);
            
            AlterColumn("dbo.Check", "QCCompletedByName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BankAccount", "BACheckTypeId", "dbo.BACheckType");
            DropForeignKey("dbo.BankAccount", "CheckId", "dbo.Check");
            DropIndex("dbo.BankAccount", new[] { "BACheckTypeId" });
            DropIndex("dbo.BankAccount", new[] { "CheckId" });
            AlterColumn("dbo.Check", "QCCompletedByName", c => c.String(nullable: false));
            DropTable("dbo.BankAccount");
            DropTable("dbo.BACheckType");
        }
    }
}
