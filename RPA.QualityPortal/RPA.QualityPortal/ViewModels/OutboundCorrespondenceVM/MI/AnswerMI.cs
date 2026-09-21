using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI
{

    [ExcludeFromCodeCoverage]
    public class AnswerMI
    {
        public Question Question { get; set; }

        [Display(Name = "Yes")]
        public int AnswerYes { get; set; }

        [Display(Name = "Yes %")]
        public decimal AnswerYesPercent { get; set; }

        [Display(Name = "No")]
        public int AnswerNo { get; set; }

        [Display(Name = "No %")]
        public decimal AnswerNoPercent { get; set; }

        [Display(Name = "N/A")]
        public int AnswerNA { get; set; }

        [Display(Name = "N/A %")]
        public decimal AnswerNAPercent { get; set; }

        [Display(Name = "Total Answers")]
        public int TotalAnswers { get; set; }
    }
}