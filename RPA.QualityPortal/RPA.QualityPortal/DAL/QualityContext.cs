using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.DAL
{
    [ExcludeFromCodeCoverage]
    public class QualityContext : DbContext, IQualityContext
    {
        public QualityContext() : base("QualityContext")
        {
            Database.CommandTimeout = 1000;
        }

        //Generic QC Items
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<Challenge> Challenge { get; set; }
        public virtual DbSet<Check> Checks { get; set; }
        public virtual DbSet<CheckQuestion> CheckQuestions { get; set; }
        public virtual DbSet<CheckResult> CheckResults { get; set; }
        public virtual DbSet<CheckType> CheckTypes { get; set; }
        public virtual DbSet<Control> Control { get; set; }
        public virtual DbSet<LockCheck> LockCheck { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Result> Results { get; set; }

        //Outbound Correspondence
        public virtual DbSet<AmendmentReason> AmendmentReasons { get; set; }
        public virtual DbSet<BusinessArea> BusinessAreas { get; set; }
        public virtual DbSet<ChallengeOutcome> ChallengeOutcomes { get; set; }
        public virtual DbSet<CheckAmendmentReason> CheckAmendmentReason { get; set; }
        public virtual DbSet<CorrespondenceType> CorrespondenceTypes { get; set; }
        public virtual DbSet<CRMRefPrefix> CRMRefPrefixes { get; set; }
        public virtual DbSet<FailReason> FailReasons { get; set; }
        public virtual DbSet<OutboundCorrespondence> OutboundCorrespondence { get; set; }
        public virtual DbSet<OutboundCorrespondenceReCheck> OutboundCorrespondenceReCheck { get; set; }
        public virtual DbSet<Scheme> Schemes { get; set; }
        public virtual DbSet<UniqueIdentifierPrefix> UniqueIdentifierPrefixes { get; set; }

        //Bank Account
        public virtual DbSet<BankAccountCheck> BankAccounts { get; set; }
        public virtual DbSet<BankAccountReCheck> BankAccountReChecks { get; set; }
        public virtual DbSet<BACheckType> BACheckTypes { get; set; }
        public virtual DbSet<BankAccountComments> BankAccountComments {get;set;}

        //Worker Service
        public virtual DbSet<WorkerServiceLog> WorkerServiceLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<OutboundCorrespondence>().ToTable("OutboundCorrespondence");
            modelBuilder.Entity<OutboundCorrespondenceReCheck>().ToTable("OutboundCorrespondenceReCheck");
            modelBuilder.Entity<BankAccountCheck>().ToTable("BankAccount");
            modelBuilder.Entity<BankAccountReCheck>().ToTable("BankAccountReCheck");
        }

        //Add this to the DAL context class
        public void SetModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified;
        }
    }
}