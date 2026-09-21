namespace RPA.QualityPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class removeCheckResultLink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmendmentReason",
                c => new
                    {
                        AmendmentReasonId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AmendmentReasonId);
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        QuestionId = c.Int(nullable: false),
                        CheckId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.Question", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.QuestionId);
            
            CreateTable(
                "dbo.Audit",
                c => new
                    {
                        AuditId = c.Int(nullable: false, identity: true),
                        CheckId = c.Guid(nullable: false),
                        DateQCUpdated = c.DateTime(),
                        Check_CheckId = c.Int(),
                    })
                .PrimaryKey(t => t.AuditId)
                .ForeignKey("dbo.Check", t => t.Check_CheckId)
                .Index(t => t.Check_CheckId);
            
            CreateTable(
                "dbo.Check",
                c => new
                    {
                        CheckId = c.Int(nullable: false, identity: true),
                        PersonName = c.String(nullable: false),
                        QCCompletedByName = c.String(nullable: false),
                        DateQCCompleted = c.DateTime(),
                        DateCreated = c.DateTime(),
                        Archived = c.Boolean(nullable: false),
                        CheckTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.CheckType", t => t.CheckTypeId, cascadeDelete: true)
                .Index(t => t.CheckTypeId);
            
            CreateTable(
                "dbo.CheckType",
                c => new
                    {
                        CheckTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CheckTypeId);
            
            CreateTable(
                "dbo.BusinessArea",
                c => new
                    {
                        BusinessAreaId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessAreaId);
            
            CreateTable(
                "dbo.CorrespondenceType",
                c => new
                    {
                        CorrespondenceTypeId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CorrespondenceTypeId);
            
            CreateTable(
                "dbo.CRMRefPrefix",
                c => new
                    {
                        CRMRefPrefixId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CRMRefPrefixId);
            
            CreateTable(
                "dbo.FailReason",
                c => new
                    {
                        FailReasonID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FailReasonID);
            
            CreateTable(
                "dbo.Scheme",
                c => new
                    {
                        SchemeId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SchemeId);
            
            CreateTable(
                "dbo.UniqueIdentifierPrefix",
                c => new
                    {
                        UniqueIdentifierPrefixId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UniqueIdentifierPrefixId);
            
            CreateTable(
                "dbo.Challenge",
                c => new
                    {
                        ChallengeId = c.Int(nullable: false, identity: true),
                        CheckId = c.Int(nullable: false),
                        ChallengeDate = c.DateTime(nullable: false),
                        ChallengeOutcome_ChallengeOutcomeId = c.Int(),
                    })
                .PrimaryKey(t => t.ChallengeId)
                .ForeignKey("dbo.ChallengeOutcome", t => t.ChallengeOutcome_ChallengeOutcomeId)
                .ForeignKey("dbo.Check", t => t.CheckId, cascadeDelete: true)
                .Index(t => t.CheckId)
                .Index(t => t.ChallengeOutcome_ChallengeOutcomeId);
            
            CreateTable(
                "dbo.ChallengeOutcome",
                c => new
                    {
                        ChallengeOutcomeId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChallengeOutcomeId);
            
            CreateTable(
                "dbo.CheckQuestion",
                c => new
                    {
                        CheckTypeId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CheckTypeId, t.QuestionId })
                .ForeignKey("dbo.CheckType", t => t.CheckTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Question", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.CheckTypeId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.CheckResultFailReason",
                c => new
                    {
                        CheckResultId = c.Int(nullable: false),
                        FailReasonId = c.Int(nullable: false),
                        CheckResult_CheckId = c.Int(),
                        CheckResult_ResultId = c.Int(),
                    })
                .PrimaryKey(t => new { t.CheckResultId, t.FailReasonId })
                .ForeignKey("dbo.CheckResult", t => new { t.CheckResult_CheckId, t.CheckResult_ResultId })
                .ForeignKey("dbo.FailReason", t => t.FailReasonId, cascadeDelete: true)
                .Index(t => t.FailReasonId)
                .Index(t => new { t.CheckResult_CheckId, t.CheckResult_ResultId });
            
            CreateTable(
                "dbo.CheckResult",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        ResultId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CheckId, t.ResultId })
                .ForeignKey("dbo.Check", t => t.CheckId, cascadeDelete: true)
                .ForeignKey("dbo.Result", t => t.ResultId, cascadeDelete: true)
                .Index(t => t.CheckId)
                .Index(t => t.ResultId);
            
            CreateTable(
                "dbo.Result",
                c => new
                    {
                        ResultId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.ResultId);
            
            CreateTable(
                "dbo.QCResult",
                c => new
                    {
                        QCResultID = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QCResultID);
            
            CreateTable(
                "dbo.OutboundCorrespondence",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        BusinessArea_BusinessAreaId = c.Int(),
                        CorrespondenceType_CorrespondenceTypeId = c.Int(),
                        CRMRefPrefix_CRMRefPrefixId = c.Int(),
                        FailReason_FailReasonID = c.Int(),
                        Scheme_SchemeId = c.Int(),
                        UniqueIdentifierPrefix_UniqueIdentifierPrefixId = c.Int(),
                        OutBoundCorresondenceId = c.Int(nullable: false),
                        TemplateReference = c.String(nullable: false),
                        UniqueId = c.Int(nullable: false),
                        SBI = c.String(nullable: false),
                        CRMRef = c.String(nullable: false),
                        OutboundCorrespondenceDateSent = c.DateTime(),
                        ManagerName = c.String(nullable: false),
                        HEO = c.String(nullable: false),
                        SEO = c.String(nullable: false),
                        Comments = c.String(),
                        ReCheckRequired = c.Boolean(nullable: false),
                        ExcludeQCResult = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Check", t => t.CheckId)
                .ForeignKey("dbo.BusinessArea", t => t.BusinessArea_BusinessAreaId)
                .ForeignKey("dbo.CorrespondenceType", t => t.CorrespondenceType_CorrespondenceTypeId)
                .ForeignKey("dbo.CRMRefPrefix", t => t.CRMRefPrefix_CRMRefPrefixId)
                .ForeignKey("dbo.FailReason", t => t.FailReason_FailReasonID)
                .ForeignKey("dbo.Scheme", t => t.Scheme_SchemeId)
                .ForeignKey("dbo.UniqueIdentifierPrefix", t => t.UniqueIdentifierPrefix_UniqueIdentifierPrefixId)
                .Index(t => t.CheckId)
                .Index(t => t.BusinessArea_BusinessAreaId)
                .Index(t => t.CorrespondenceType_CorrespondenceTypeId)
                .Index(t => t.CRMRefPrefix_CRMRefPrefixId)
                .Index(t => t.FailReason_FailReasonID)
                .Index(t => t.Scheme_SchemeId)
                .Index(t => t.UniqueIdentifierPrefix_UniqueIdentifierPrefixId);
            
            CreateTable(
                "dbo.OutboundCorrespondenceReCheck",
                c => new
                    {
                        CheckId = c.Int(nullable: false),
                        OutboundCorrespondence_CheckId = c.Int(),
                        OutboundCorrespondenceReCheckId = c.Int(nullable: false),
                        OutBoundCorresondenceId = c.Int(nullable: false),
                        ReCheckActive = c.Boolean(nullable: false),
                        ReCheckComments = c.String(),
                        FurtherReCheckRequired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.Check", t => t.CheckId)
                .ForeignKey("dbo.OutboundCorrespondence", t => t.OutboundCorrespondence_CheckId)
                .Index(t => t.CheckId)
                .Index(t => t.OutboundCorrespondence_CheckId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutboundCorrespondenceReCheck", "OutboundCorrespondence_CheckId", "dbo.OutboundCorrespondence");
            DropForeignKey("dbo.OutboundCorrespondenceReCheck", "CheckId", "dbo.Check");
            DropForeignKey("dbo.OutboundCorrespondence", "UniqueIdentifierPrefix_UniqueIdentifierPrefixId", "dbo.UniqueIdentifierPrefix");
            DropForeignKey("dbo.OutboundCorrespondence", "Scheme_SchemeId", "dbo.Scheme");
            DropForeignKey("dbo.OutboundCorrespondence", "FailReason_FailReasonID", "dbo.FailReason");
            DropForeignKey("dbo.OutboundCorrespondence", "CRMRefPrefix_CRMRefPrefixId", "dbo.CRMRefPrefix");
            DropForeignKey("dbo.OutboundCorrespondence", "CorrespondenceType_CorrespondenceTypeId", "dbo.CorrespondenceType");
            DropForeignKey("dbo.OutboundCorrespondence", "BusinessArea_BusinessAreaId", "dbo.BusinessArea");
            DropForeignKey("dbo.OutboundCorrespondence", "CheckId", "dbo.Check");
            DropForeignKey("dbo.CheckResultFailReason", "FailReasonId", "dbo.FailReason");
            DropForeignKey("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" }, "dbo.CheckResult");
            DropForeignKey("dbo.CheckResult", "ResultId", "dbo.Result");
            DropForeignKey("dbo.CheckResult", "CheckId", "dbo.Check");
            DropForeignKey("dbo.CheckQuestion", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.CheckQuestion", "CheckTypeId", "dbo.CheckType");
            DropForeignKey("dbo.Challenge", "CheckId", "dbo.Check");
            DropForeignKey("dbo.Challenge", "ChallengeOutcome_ChallengeOutcomeId", "dbo.ChallengeOutcome");
            DropForeignKey("dbo.Audit", "Check_CheckId", "dbo.Check");
            DropForeignKey("dbo.Check", "CheckTypeId", "dbo.CheckType");
            DropForeignKey("dbo.Answer", "QuestionId", "dbo.Question");
            DropIndex("dbo.OutboundCorrespondenceReCheck", new[] { "OutboundCorrespondence_CheckId" });
            DropIndex("dbo.OutboundCorrespondenceReCheck", new[] { "CheckId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "UniqueIdentifierPrefix_UniqueIdentifierPrefixId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "Scheme_SchemeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "FailReason_FailReasonID" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CRMRefPrefix_CRMRefPrefixId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CorrespondenceType_CorrespondenceTypeId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "BusinessArea_BusinessAreaId" });
            DropIndex("dbo.OutboundCorrespondence", new[] { "CheckId" });
            DropIndex("dbo.CheckResult", new[] { "ResultId" });
            DropIndex("dbo.CheckResult", new[] { "CheckId" });
            DropIndex("dbo.CheckResultFailReason", new[] { "CheckResult_CheckId", "CheckResult_ResultId" });
            DropIndex("dbo.CheckResultFailReason", new[] { "FailReasonId" });
            DropIndex("dbo.CheckQuestion", new[] { "QuestionId" });
            DropIndex("dbo.CheckQuestion", new[] { "CheckTypeId" });
            DropIndex("dbo.Challenge", new[] { "ChallengeOutcome_ChallengeOutcomeId" });
            DropIndex("dbo.Challenge", new[] { "CheckId" });
            DropIndex("dbo.Check", new[] { "CheckTypeId" });
            DropIndex("dbo.Audit", new[] { "Check_CheckId" });
            DropIndex("dbo.Answer", new[] { "QuestionId" });
            DropTable("dbo.OutboundCorrespondenceReCheck");
            DropTable("dbo.OutboundCorrespondence");
            DropTable("dbo.QCResult");
            DropTable("dbo.Result");
            DropTable("dbo.CheckResult");
            DropTable("dbo.CheckResultFailReason");
            DropTable("dbo.CheckQuestion");
            DropTable("dbo.ChallengeOutcome");
            DropTable("dbo.Challenge");
            DropTable("dbo.UniqueIdentifierPrefix");
            DropTable("dbo.Scheme");
            DropTable("dbo.FailReason");
            DropTable("dbo.CRMRefPrefix");
            DropTable("dbo.CorrespondenceType");
            DropTable("dbo.BusinessArea");
            DropTable("dbo.CheckType");
            DropTable("dbo.Check");
            DropTable("dbo.Audit");
            DropTable("dbo.Question");
            DropTable("dbo.Answer");
            DropTable("dbo.AmendmentReason");
        }
    }
}
