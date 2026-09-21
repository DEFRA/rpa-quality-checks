using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models.CheckTypes
{
    [ExcludeFromCodeCoverage]
    public class BankAccountReCheck : Check
    {
        public int BankAccountId { get; set; }

        public bool ReCheckActive { get; set; }

        [Display(Name = "Recheck Comments")]
        public string ReCheckComments { get; set; }

        [Display(Name = "Recheck Completed By?")]
        [Required]
        public string ReCheckCompletedBy { get; set; }

        [Display(Name = "Is a Further Recheck Required?")]
        public bool FurtherReCheckRequired { get; set; }

        [Display(Name = "Recheck Coaching Point")]
        public bool ReCheckCoachingPoint { get; set; }

        public int? FailReasonId { get; set; }

        public virtual FailReason FailReason { get; set; }

        public BankAccountReCheck()
        {
            CheckTypeId = 4;
            EmailNotifications = true;
            FurtherReCheckRequired = false;
        }

        public BankAccountReCheck(int CheckId) : this()
        {
            BankAccountId = CheckId;
        }
    }
}