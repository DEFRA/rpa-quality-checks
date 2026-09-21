using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class Check
    {
        [Key]
        public int CheckId { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string PersonName { get; set; }

        [Display(Name = "QC Completed By")]
        public string QCCompletedByName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date QC Completed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime? DateQCCompleted { get; set; }

        public bool Archived { get; set; }

        public int CheckTypeId { get; set; }

        public virtual CheckType CheckType { get; set; }

        [Display(Name = "Send Email Notification")]
        public bool EmailNotifications { get; set; }
    }
}