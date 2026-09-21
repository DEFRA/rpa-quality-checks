using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Diagnostics.CodeAnalysis;


namespace RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI
{
    [ExcludeFromCodeCoverage]
    public class FailReasonCount
    {
        [Display(Name = "Not Approved Reason")]
        public string FailReason { get; set; }

        [Display(Name = "Number of QC's Not Approved")]
        public int FailedNumber { get; set; }

        [Display(Name = "% of QC's Not Approved")]
        public decimal FailedPercentage { get; set; }
    }
}