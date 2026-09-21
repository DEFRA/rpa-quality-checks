namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class BankReCheckModelBuilder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccountReCheck",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                        ReCheckActive = c.Boolean(nullable: false),
                        ReCheckComments = c.String(),
                        ReCheckCompletedBy = c.String(nullable: false),
                        FurtherReCheckRequired = c.Boolean(nullable: false),
                        ReCheckCoachingPoint = c.Boolean(nullable: false),
                        FailReasonId = c.Int(),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Check", t => t.CheckId)
                .ForeignKey("dbo.FailReason", t => t.FailReasonId)
                .Index(t => t.CheckId)
                .Index(t => t.FailReasonId);
            
            DropColumn("dbo.Check", "BankAccountId");
            DropColumn("dbo.Check", "ReCheckActive");
            DropColumn("dbo.Check", "ReCheckComments");
            DropColumn("dbo.Check", "ReCheckCompletedBy");
            DropColumn("dbo.Check", "FurtherReCheckRequired");
            DropColumn("dbo.Check", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Check", "Discriminator", c => c.String(maxLength: 128));
            AddColumn("dbo.Check", "FurtherReCheckRequired", c => c.Boolean());
            AddColumn("dbo.Check", "ReCheckCompletedBy", c => c.String());
            AddColumn("dbo.Check", "ReCheckComments", c => c.String());
            AddColumn("dbo.Check", "ReCheckActive", c => c.Boolean());
            AddColumn("dbo.Check", "BankAccountId", c => c.Int());
            DropForeignKey("dbo.BankAccountReCheck", "FailReasonId", "dbo.FailReason");
            DropForeignKey("dbo.BankAccountReCheck", "CheckId", "dbo.Check");
            DropIndex("dbo.BankAccountReCheck", new[] { "FailReasonId" });
            DropIndex("dbo.BankAccountReCheck", new[] { "CheckId" });
            DropTable("dbo.BankAccountReCheck");
        }
    }
}
