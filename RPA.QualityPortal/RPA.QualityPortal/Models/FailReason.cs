using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class FailReason
    {
        public int FailReasonId { get; set; }

        [Display(Name = "Not Approved Reason")]
        public string Text { get; set; }

        public bool Active { get; set; }

        public int? CheckTypeId { get; set; }

        public virtual CheckType CheckType { get; set; }
    }
}