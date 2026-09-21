using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models.BankAccountDropDowns
{
    [ExcludeFromCodeCoverage]
    public class BACheckType
    {
        [Key]
        public int BACheckTypeId { get; set; }

        [Display(Name = "Check Type")]
        public string Text { get; set; }

        public bool Active { get; set; }
    }
}