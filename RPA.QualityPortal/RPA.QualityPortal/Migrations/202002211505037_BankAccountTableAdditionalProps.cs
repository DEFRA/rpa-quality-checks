namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class BankAccountTableAdditionalProps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccount", "Comments", c => c.String());
            AddColumn("dbo.BankAccount", "FailReasonId", c => c.Int());
            AddColumn("dbo.BankAccount", "ReCheckRequired", c => c.Boolean(nullable: false));
            CreateIndex("dbo.BankAccount", "FailReasonId");
            AddForeignKey("dbo.BankAccount", "FailReasonId", "dbo.FailReason", "FailReasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BankAccount", "FailReasonId", "dbo.FailReason");
            DropIndex("dbo.BankAccount", new[] { "FailReasonId" });
            DropColumn("dbo.BankAccount", "ReCheckRequired");
            DropColumn("dbo.BankAccount", "FailReasonId");
            DropColumn("dbo.BankAccount", "Comments");
        }
    }
}
