using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels.BankAccount
{
    [ExcludeFromCodeCoverage]
    public class BankAccountDetailsReCheck
    {
        public BankAccountCheck BankAccount { get; set; }

        public BankAccountReCheck BankAccountReCheck { get; set; }

        public List<Answer> Answers { get; set; }

        public List<Answer> ReCheckAnswers { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string ReCheckResult { get; set; }

        public List<BankAccountComments> Comments { get; set; }

        public List<BankAccountComments> ReCheckComments { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public CheckAmendmentReason ReCheckAmendmentReason { get; set; }
    }
}