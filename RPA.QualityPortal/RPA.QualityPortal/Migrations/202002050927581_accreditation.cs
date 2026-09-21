namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]

    public partial class accreditation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OutboundCorrespondenceAccreditation",
                c => new
                    {
                        OutboundCorrespondenceAccreditationId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SendFunctionInCRM = c.Boolean(nullable: false),
                        NoOfConsecutivePassesBpsEmail = c.Int(nullable: false),
                        AccreditationBpsEmail = c.Boolean(nullable: false),
                        NoOfConsecutivePassesBpsLetter = c.Int(nullable: false),
                        AccreditationBpsLetter = c.Boolean(nullable: false),
                        NoOfConsecutivePassesCsEmail = c.Int(nullable: false),
                        AccreditationCsEmail = c.Boolean(nullable: false),
                        NoOfConsecutivePassesCsLetter = c.Int(nullable: false),
                        AccreditationCsLetter = c.Boolean(nullable: false),
                        NoOfConsecutivePassesEsEmail = c.Int(nullable: false),
                        AccreditationEsEmail = c.Boolean(nullable: false),
                        NoOfConsecutivePassesEsLetter = c.Int(nullable: false),
                        AccreditationEsLetter = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OutboundCorrespondenceAccreditationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OutboundCorrespondenceAccreditation");
        }
    }
}
