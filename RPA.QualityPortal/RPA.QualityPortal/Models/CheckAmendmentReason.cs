using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models
{
    [ExcludeFromCodeCoverage]
    public class CheckAmendmentReason
    {
        [Key]
        public int CheckAmendmentReasonId { get; set; }

        public int CheckId { get; set; }

        public int AmendmentReasonId { get; set; }

        [Display(Name = "Amendment Reason Comments")]
        public string AmendmentReasonComments { get; set; }

        public virtual Check Check { get; set; }

        public virtual AmendmentReason AmendmentReason { get; set; }
    }
}