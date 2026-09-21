namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class challengeOutcomeID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Challenge", "ChallengeOutcome_ChallengeOutcomeId", "dbo.ChallengeOutcome");
            DropIndex("dbo.Challenge", new[] { "ChallengeOutcome_ChallengeOutcomeId" });
            RenameColumn(table: "dbo.Challenge", name: "ChallengeOutcome_ChallengeOutcomeId", newName: "ChallengeOutcomeId");
            AlterColumn("dbo.Challenge", "ChallengeOutcomeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Challenge", "ChallengeOutcomeId");
            AddForeignKey("dbo.Challenge", "ChallengeOutcomeId", "dbo.ChallengeOutcome", "ChallengeOutcomeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Challenge", "ChallengeOutcomeId", "dbo.ChallengeOutcome");
            DropIndex("dbo.Challenge", new[] { "ChallengeOutcomeId" });
            AlterColumn("dbo.Challenge", "ChallengeOutcomeId", c => c.Int());
            RenameColumn(table: "dbo.Challenge", name: "ChallengeOutcomeId", newName: "ChallengeOutcome_ChallengeOutcomeId");
            CreateIndex("dbo.Challenge", "ChallengeOutcome_ChallengeOutcomeId");
            AddForeignKey("dbo.Challenge", "ChallengeOutcome_ChallengeOutcomeId", "dbo.ChallengeOutcome", "ChallengeOutcomeId");
        }
    }
}
