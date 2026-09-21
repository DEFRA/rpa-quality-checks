using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BankAccountOverview
    {
        public BankAccountCheck BankAccount { get; set; }

        [Display(Name = "Days Elapsed")]
        public int? DaysElapsed {
            get 
            {
                if (BankAccount.DateQCCompleted == null)
                {
                    return (int)(DateTime.Now - BankAccount.DateQCCreated).TotalDays;
                }

                return null;
            }
        }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC Recheck Result")]
        public string ReCheckQCResult { get; set; }

        public BankAccountReCheck BankAccountReCheck { get; set; }
    }
}