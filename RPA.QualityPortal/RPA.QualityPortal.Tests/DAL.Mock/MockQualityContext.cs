using Moq;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Tests.Data.Mock;
using System.Data.Entity;

namespace RPA.QualityPortal.Tests.DAL.Mock
{
    public class MockQualityContext
    {
        public Mock<IQualityContext> MockContext { get; set; }

        //Generic QC Items
        public virtual Mock<DbSet<Answer>> MockAnswers { get; set; }
        public virtual Mock<DbSet<Audit>> MockAudits { get; set; }
        public virtual Mock<DbSet<Challenge>> MockChallenge { get; set; }
        public virtual Mock<DbSet<Check>> MockChecks { get; set; }
        public virtual Mock<DbSet<CheckQuestion>> MockCheckQuestions { get; set; }
        public virtual Mock<DbSet<CheckResult>> MockCheckResults { get; set; }
        public virtual Mock<DbSet<CheckType>> MockCheckTypes { get; set; }
        public virtual Mock<DbSet<Control>> MockControl { get; set; }
        public virtual Mock<DbSet<LockCheck>> MockLockCheck { get; set; }
        public virtual Mock<DbSet<Question>> MockQuestions { get; set; }
        public virtual Mock<DbSet<Result>> MockResults { get; set; }

        //Outbound Correspondence
        public virtual Mock<DbSet<AmendmentReason>> MockAmendmentReasons { get; set; }
        public virtual Mock<DbSet<BusinessArea>> MockBusinessAreas { get; set; }
        public virtual Mock<DbSet<ChallengeOutcome>> MockChallengeOutcomes { get; set; }
        public virtual Mock<DbSet<CheckAmendmentReason>> MockCheckAmendmentReason { get; set; }
        public virtual Mock<DbSet<CorrespondenceType>> MockCorrespondenceTypes { get; set; }
        public virtual Mock<DbSet<CRMRefPrefix>> MockCRMRefPrefixes { get; set; }
        public virtual Mock<DbSet<FailReason>> MockFailReasons { get; set; }
        public virtual Mock<DbSet<OutboundCorrespondence>> MockOutboundCorrespondence { get; set; }
        public virtual Mock<DbSet<OutboundCorrespondenceReCheck>> MockOutboundCorrespondenceReCheck { get; set; }
        public virtual Mock<DbSet<Scheme>> MockSchemes { get; set; }
        public virtual Mock<DbSet<UniqueIdentifierPrefix>> MockUniqueIdentifierPrefixes { get; set; }

        //Bank Account
        public virtual Mock<DbSet<BankAccountCheck>> MockBankAccounts { get; set; }
        public virtual Mock<DbSet<BACheckType>> MockBACheckTypes { get; set; }
        public virtual Mock<DbSet<BankAccountReCheck>> MockBankAccountReChecks { get; set; }
        public virtual Mock<DbSet<BankAccountComments>> MockBankAccountComments { get; set; }

        public MockQualityContext(bool setMocks = true)
        {
            if (setMocks)
            {
                SetMocks();
            }
        }

        public void SetMocks()
        {
            SetMockContext();
            SetMockAnswers();       
            SetMockAudits();
            SetMockChecks();
            SetMockCheckQuestions();
            SetMockCheckResults();
            SetMockCheckTypes();
            SetMockResults();
            SetMockChallenge();
            SetMockAmendmentReasons();
            SetMockBusinessAreas();
            SetMockChallengeOutcomes();
            SetMockCorrespondenceTypes();
            SetMockCRMRefPrefixes();
            SetMockFailReasons();
            SetMockSchemes();
            SetMockUniqueIdentifierPrefixes();
            SetMockOutboundCorrespondence();
            SetMockOutboundCorrespondenceReCheck();
            SetMockCheckAmendmentReason();
            SetMockQuestions();
            SetMockControl();
            SetMockLockCheck();
            SetMockBACheckTypes();
            SetMockBankAccounts();
            SetMockBankAccountReChecks();
            SetMockBankAccountComments();
        }

        private void SetMockBankAccountReChecks()
        {
            MockBankAccountReChecks = new Mock<DbSet<BankAccountReCheck>>().SetupData(BankAccountReCheckData.Data());
            MockContext.Setup(x => x.BankAccountReChecks).Returns(MockBankAccountReChecks.Object);
        }

        private void SetMockBACheckTypes()
        {
            MockBACheckTypes = new Mock<DbSet<BACheckType>>().SetupData(BACheckTypeData.Data());
            MockContext.Setup(x => x.BACheckTypes).Returns(MockBACheckTypes.Object);
        }

        private void SetMockBankAccounts()
        {
            MockBankAccounts = new Mock<DbSet<BankAccountCheck>>().SetupData(BankAccountData.Data());
            MockContext.Setup(x => x.BankAccounts).Returns(MockBankAccounts.Object);
        }

        public void SetMockOutboundCorrespondence()
        {
            MockOutboundCorrespondence = new Mock<DbSet<OutboundCorrespondence>>().SetupData(OutboundCorrespondenceData.Data());
            MockContext.Setup(x => x.OutboundCorrespondence).Returns(MockOutboundCorrespondence.Object);
        }

        public void SetMockOutboundCorrespondenceReCheck()
        {
            MockOutboundCorrespondenceReCheck = new Mock<DbSet<OutboundCorrespondenceReCheck>>().SetupData(OutboundCorrespondenceReCheckData.Data());
            MockContext.Setup(x => x.OutboundCorrespondenceReCheck).Returns(MockOutboundCorrespondenceReCheck.Object);
        }

        public void SetMockCheckAmendmentReason()
        {
            MockCheckAmendmentReason = new Mock<DbSet<CheckAmendmentReason>>().SetupData(CheckAmendmentReasonData.Data());
            MockContext.Setup(x => x.CheckAmendmentReason).Returns(MockCheckAmendmentReason.Object);
        }

        public void SetMockAnswers()
        {
            MockAnswers = new Mock<DbSet<Answer>>().SetupData(AnswerData.Data());
            MockContext.Setup(x => x.Answers).Returns(MockAnswers.Object);
        }

        public void SetMockSchemes()
        {
            MockSchemes = new Mock<DbSet<Scheme>>().SetupData(SchemeData.Data());
            MockContext.Setup(x => x.Schemes).Returns(MockSchemes.Object);
        }

        public void SetMockBusinessAreas()
        {
            MockBusinessAreas = new Mock<DbSet<BusinessArea>>().SetupData(BusinessAreaData.Data());
            MockContext.Setup(x => x.BusinessAreas).Returns(MockBusinessAreas.Object);
        }

        public void SetMockFailReasons()
        {
            MockFailReasons = new Mock<DbSet<FailReason>>().SetupData(FailReasonData.Data());
            MockContext.Setup(x => x.FailReasons).Returns(MockFailReasons.Object);
        }

        public void SetMockAudits()
        {
            MockAudits = new Mock<DbSet<Audit>>().SetupData(AuditData.Data());
            MockContext.Setup(x => x.Audits).Returns(MockAudits.Object);
        }

        public void SetMockChecks()
        {
            MockChecks = new Mock<DbSet<Check>>().SetupData(CheckData.Data());
            MockContext.Setup(x => x.Checks).Returns(MockChecks.Object);
        }

        public void SetMockCRMRefPrefixes()
        {
            MockCRMRefPrefixes = new Mock<DbSet<CRMRefPrefix>>().SetupData(CRMRefPrefixData.Data());
            MockContext.Setup(x => x.CRMRefPrefixes).Returns(MockCRMRefPrefixes.Object);
        }

        public void SetMockUniqueIdentifierPrefixes()
        {
            MockUniqueIdentifierPrefixes = new Mock<DbSet<UniqueIdentifierPrefix>>().SetupData(UniqueIdentifierPrefixData.Data());
            MockContext.Setup(x => x.UniqueIdentifierPrefixes).Returns(MockUniqueIdentifierPrefixes.Object);
        }

        public void SetMockCorrespondenceTypes()
        {
            MockCorrespondenceTypes = new Mock<DbSet<CorrespondenceType>>().SetupData(CorrespondenceTypeData.Data());
            MockContext.Setup(x => x.CorrespondenceTypes).Returns(MockCorrespondenceTypes.Object);
        }

        public void SetMockChallenge()
        {
            MockChallenge = new Mock<DbSet<Challenge>>().SetupData(ChallengeData.Data());
            MockContext.Setup(x => x.Challenge).Returns(MockChallenge.Object);
        }

        public void SetMockChallengeOutcomes()
        {
            MockChallengeOutcomes = new Mock<DbSet<ChallengeOutcome>>().SetupData(ChallengeOutcomeData.Data());
            MockContext.Setup(x => x.ChallengeOutcomes).Returns(MockChallengeOutcomes.Object);
        }

        public void SetMockAmendmentReasons()
        {
            MockAmendmentReasons = new Mock<DbSet<AmendmentReason>>().SetupData(AmendmentReasonData.Data());
            MockContext.Setup(x => x.AmendmentReasons).Returns(MockAmendmentReasons.Object);
        }

        public void SetMockCheckTypes()
        {
            MockCheckTypes = new Mock<DbSet<CheckType>>().SetupData(CheckTypeData.Data());
            MockContext.Setup(x => x.CheckTypes).Returns(MockCheckTypes.Object);
        }

        public void SetMockCheckResults()
        {
            MockCheckResults = new Mock<DbSet<CheckResult>>().SetupData(CheckResultData.Data());
            MockContext.Setup(x => x.CheckResults).Returns(MockCheckResults.Object);
        }

        public void SetMockResults()
        {
            MockResults = new Mock<DbSet<Result>>().SetupData(ResultData.Data());
            MockContext.Setup(x => x.Results).Returns(MockResults.Object);
        }

        public void SetMockCheckQuestions()
        {
            MockCheckQuestions = new Mock<DbSet<CheckQuestion>>().SetupData(CheckQuestionData.Data());
            MockContext.Setup(x => x.CheckQuestions).Returns(MockCheckQuestions.Object);
        }

        public void SetMockQuestions()
        {
            MockQuestions = new Mock<DbSet<Question>>().SetupData(QuestionData.Data());
            MockContext.Setup(x => x.Questions).Returns(MockQuestions.Object);
        }

        public void SetMockControl()
        {
            MockControl = new Mock<DbSet<Control>>().SetupData(ControlData.Data());
            MockContext.Setup(x => x.Control).Returns(MockControl.Object);
        }

        public void SetMockLockCheck()
        {
            MockLockCheck = new Mock<DbSet<LockCheck>>().SetupData(LockCheckData.Data());
            MockContext.Setup(x => x.LockCheck).Returns(MockLockCheck.Object);
        }

        public void SetMockBankAccountComments()
        {
            MockBankAccountComments = new Mock<DbSet<BankAccountComments>>().SetupData(BankAccountCommentData.Data());
            MockContext.Setup(x => x.BankAccountComments).Returns(MockBankAccountComments.Object);
        }

        public void SetMockContext()
        {
            MockContext = new Mock<IQualityContext>();
        }
    }
}
