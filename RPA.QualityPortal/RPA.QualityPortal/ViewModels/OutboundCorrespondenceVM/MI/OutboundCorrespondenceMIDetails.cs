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
    public class OutboundCorrespondenceMIDetails
    {
        [Display(Name = "Not Approved")]
        public int Failed { get; set; }

        [Display(Name = "Not Approved %")]
        public decimal FailedPercentage { get; set; }

        [Display(Name = "From: ")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "To: ")]
        public DateTime? DateTo { get; set; }

        public List<FailReasonCount> FailReasonCount { get; set; }

        public string SearchType { get; set; }

        public string ResultHeader { get; set; }

        public List<OutboundCorrespondenceMIPersonStats> OutboundCorrespondenceMIPersonStatsList { get; set; }

        public List<AnswerMI> AnswerMIStats { get; set;}
    }
}