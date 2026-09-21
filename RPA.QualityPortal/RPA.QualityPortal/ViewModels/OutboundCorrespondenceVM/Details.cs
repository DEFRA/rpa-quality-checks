using System.Collections.Generic;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{

    [ExcludeFromCodeCoverage]
    public class Details
    {
        public OutboundCorrespondence OutboundCorrespondence { get; set; }

        public List<Answer> Answers { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public bool Challenge { get; set; }

        public Details()
        {
            Challenge = false;
        }
    }
}