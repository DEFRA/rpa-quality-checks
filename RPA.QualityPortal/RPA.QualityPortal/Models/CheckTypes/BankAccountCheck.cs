using RPA.QualityPortal.Attributes;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models.CheckTypes
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class BankAccountCheck : Check
    {
        [SBI]
        public string SBI { get; set; }

        [Required]
        public int FRN { get; set; } //DAX

        public int? BACheckTypeId { get; set; }

        [Display(Name = "Business Name")]
        [Required]
        public string BusinessName { get; set; } //DAX

        [Display(Name = "Line Manager")]
        [Required]
        public string ManagerName { get; set; } //DAX

        [DataType(DataType.Date)]
        [Display(Name = "Date QC Created")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy HH:mm}")]
        public DateTime DateQCCreated { get; set; } // DAX

        public string Comments { get; set; }

        [Display(Name = "Coaching Point")]
        public bool CoachingPoint { get; set; }

        public int? FailReasonId { get; set; }

        [Display(Name = "Recheck Required")]
        public bool ReCheckRequired { get; set; }

        [Display(Name = "Initial Recheck Decision")]
        public bool InitialReCheckDecision { get; set; }

        [Display(Name = "Override Check")]
        public bool Overriden { get; set; }

        public virtual BACheckType BACheckType { get; set; }

        public virtual FailReason FailReason { get; set; }


    }
}
