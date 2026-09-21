using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace RPA.QualityPortal.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LockCheckView
    {
        public LockCheck LockCheck { get; set; }

        public OutboundCorrespondence OutboundCorrespondence { get; set; }

        public BankAccountCheck BankAccount { get; set; }

        [Display(Name = "Recheck")]
        public bool ReCheck { get; set; }
    }
}