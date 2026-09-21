using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels.BankAccount
{
    [ExcludeFromCodeCoverage]
    public class BankAccountDetailsReChecks
    {
        public BankAccountCheck BankAccount { get; set; }

        public BankAccountReCheck FirstBankAccountReCheck { get; set; }

        public BankAccountReCheck SecondBankAccountReCheck { get; set; }

        public BankAccountReCheck ThirdBankAccountReCheck { get; set; }

        public List<Answer> Answers { get; set; }

        public List<Answer> FirstReCheckAnswers { get; set; }

        public List<Answer> SecondReCheckAnswers { get; set; }

        public List<Answer> ThirdReCheckAnswers { get; set; }

        public List<BankAccountComments> Comments { get; set; }

        public List<BankAccountComments> FirstReCheckComments { get; set; }

        public List<BankAccountComments> SecondReCheckComments { get; set; }

        public List<BankAccountComments> ThirdReCheckComments { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string FirstReCheckResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string SecondReCheckResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string ThirdReCheckResult { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public CheckAmendmentReason FirstReCheckAmendmentReason { get; set; }

        public CheckAmendmentReason SecondReCheckAmendmentReason { get; set; }

        public CheckAmendmentReason ThirdReCheckAmendmentReason { get; set; }
    }
}