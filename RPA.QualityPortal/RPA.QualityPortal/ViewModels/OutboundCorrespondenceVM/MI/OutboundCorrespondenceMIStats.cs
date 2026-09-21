using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI
{
    [ExcludeFromCodeCoverage]
    public class OutboundCorrespondenceMIStats
    {
        [Display(Name = "Total Completed")]
        public int TotalQcs { get; set; }

        public int Days { get; set; }

        [Display(Name = "Approved")]
        public int Passed { get; set; }

        [Display(Name = "Approved %")]
        public decimal PassedPercentage { get; set; }

        [Display(Name = "Approved Advisory")]
        public int PassAdvisory { get; set; }

        [Display(Name = "Approved Advisory %")]
        public decimal PassedAdvisoryPercentage { get; set; }

        [Display(Name = "Not Approved")]
        public int Failed { get; set; }

        [Display(Name = "Not Approved %")]
        public decimal FailedPercentage { get; set; }

        [Display(Name = "Outstanding 0-5 Days")]
        public int OutstandingZeroToFive { get; set; }

        [Display(Name = "Outstanding 6-10 Days")]
        public int OutstandingSixToTen { get; set; }

        [Display(Name = "Outstanding 11+ Days")]
        public int OutstandingGreaterThanTen { get; set; }

        [Display(Name = "Total Outstanding")]
        public int QcsOutstanding { get; set; }

        [Display(Name = "Total Completed")]
        public int ReChecks { get; set; }

        [Display(Name = "Approved")]
        public int ReCheckPassed { get; set; }

        [Display(Name = "Approved %")]
        public decimal ReCheckPassedPercentage { get; set; }

        [Display(Name = "Approved Advisory")]
        public int ReCheckPassAdvisory { get; set; }

        [Display(Name = "Approved %")]
        public decimal ReCheckPassedAdvisoryPercentage { get; set; }

        [Display(Name = "Not Approved")]
        public int ReCheckFailed { get; set; }

        [Display(Name = "Not Approved %")]
        public decimal ReCheckFailedPercentage { get; set; }

        [Display(Name = "Outstanding 0-5 Days")]
        public int ReCheckOutstandingZeroToFive { get; set; }

        [Display(Name = "Outstanding 6-10 Days")]
        public int ReCheckOutstandingSixToTen { get; set; }

        [Display(Name = "Outstanding 11+ Days")]
        public int ReCheckOutstandingGreaterThanTen { get; set; }

        [Display(Name = "Total Outstanding")]
        public int ReChecksOutstanding { get; set; }

        [Display(Name = "Recheck Required")]
        public int ReChecksRequired { get; set; }

        [Display(Name = "Recheck Not Required")]
        public int ReChecksNotRequired { get; set; }


    }
}