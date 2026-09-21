using System.Data.Entity;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;

namespace RPA.QualityPortal.DAL
{
    public interface IQualityContext
    {
        //Generic QC Items
        DbSet<Answer> Answers { get; set; }
        DbSet<Audit> Audits { get; set; }
        DbSet<Challenge> Challenge { get; set; }
        DbSet<Check> Checks { get; set; }
        DbSet<CheckQuestion> CheckQuestions { get; set; }
        DbSet<CheckResult> CheckResults { get; set; }
        DbSet<CheckType> CheckTypes { get; set; }
        DbSet<Control> Control { get; set; }
        DbSet<LockCheck> LockCheck { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<Result> Results { get; set; }

        //Outbound Correspondence
        DbSet<AmendmentReason> AmendmentReasons { get; set; }
        DbSet<BusinessArea> BusinessAreas { get; set; }
        DbSet<ChallengeOutcome> ChallengeOutcomes { get; set; }
        DbSet<CheckAmendmentReason> CheckAmendmentReason { get; set; }
        DbSet<CorrespondenceType> CorrespondenceTypes { get; set; }
        DbSet<CRMRefPrefix> CRMRefPrefixes { get; set; }
        DbSet<FailReason> FailReasons { get; set; }
        DbSet<OutboundCorrespondence> OutboundCorrespondence { get; set; }
        DbSet<OutboundCorrespondenceReCheck> OutboundCorrespondenceReCheck { get; set; }
        DbSet<Scheme> Schemes { get; set; }
        DbSet<UniqueIdentifierPrefix> UniqueIdentifierPrefixes { get; set; }

        //Bank Account
        DbSet<BankAccountCheck> BankAccounts { get; set; }
        DbSet<BankAccountReCheck> BankAccountReChecks { get; set; }
        DbSet<BACheckType> BACheckTypes { get; set; }
        DbSet<BankAccountComments> BankAccountComments { get; set; }

        void SetModified(object entity);

        int SaveChanges();

        void Dispose();
    }
}