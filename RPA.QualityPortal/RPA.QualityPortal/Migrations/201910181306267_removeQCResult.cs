namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeQCResult : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.QCResult");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QCResult",
                c => new
                    {
                        QCResultID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QCResultID);
            
        }
    }
}
