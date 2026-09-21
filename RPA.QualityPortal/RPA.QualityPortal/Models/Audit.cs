using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class Audit
    {
        [Key]
        public int AuditId { get; set; }

        public int CheckId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date QC Updated")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime? DateQCUpdated { get; set; }

        public virtual Check Check { get; set; }

        public virtual AmendmentReason AmendmentReason { get; set; }

        public string Comments { get; set; }

        public Audit()
        {
            DateQCUpdated = DateTime.Now;
        }
    }
}