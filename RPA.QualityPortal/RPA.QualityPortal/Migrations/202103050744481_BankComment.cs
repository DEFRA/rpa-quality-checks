namespace RPA.QualityPortal.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class BankComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccountComments",
                c => new
                    {
                        BankAccountCommentId = c.Int(nullable: false, identity: true),
                        AnswerComment = c.String(),
                        AnswerId = c.Int(nullable: false),
                        CheckId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BankAccountCommentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BankAccountComments");
        }
    }
}
