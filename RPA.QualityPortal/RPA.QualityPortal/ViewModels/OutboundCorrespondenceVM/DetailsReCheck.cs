using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{

    [ExcludeFromCodeCoverage]
    public class DetailsReCheck
    {
        public OutboundCorrespondence OutboundCorrespondence { get; set; }

        public OutboundCorrespondenceReCheck OutboundCorrespondenceReCheck { get; set; }

        public List<Answer> Answers { get; set; }

        public List<Answer> ReCheckAnswers { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC ReCheck Result")]
        public string ReCheckResult { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public CheckAmendmentReason ReCheckAmendmentReason { get; set; }

        public bool Challenge { get; set; }
        public DetailsReCheck()
        {
            Challenge = false;
        }
    }
}