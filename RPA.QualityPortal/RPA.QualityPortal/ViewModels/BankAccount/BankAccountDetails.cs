using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BankAccountDetails
    {
        public BankAccountCheck BankAccount { get; set; }

        public BankAccountReCheck BankAccountReCheck { get; set; }

        public List<Answer> Answers { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        public bool Challenge { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public BankAccountDetails()
        {
            Challenge = false;
        }
    }
}