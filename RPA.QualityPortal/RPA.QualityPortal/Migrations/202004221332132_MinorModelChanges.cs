namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class MinorModelChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BankAccount", "BACheckTypeId", "dbo.BACheckType");
            DropIndex("dbo.BankAccount", new[] { "BACheckTypeId" });
            AlterColumn("dbo.BankAccount", "BACheckTypeId", c => c.Int());
            CreateIndex("dbo.BankAccount", "BACheckTypeId");
            AddForeignKey("dbo.BankAccount", "BACheckTypeId", "dbo.BACheckType", "BACheckTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BankAccount", "BACheckTypeId", "dbo.BACheckType");
            DropIndex("dbo.BankAccount", new[] { "BACheckTypeId" });
            AlterColumn("dbo.BankAccount", "BACheckTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.BankAccount", "BACheckTypeId");
            AddForeignKey("dbo.BankAccount", "BACheckTypeId", "dbo.BACheckType", "BACheckTypeId", cascadeDelete: true);
        }
    }
}
