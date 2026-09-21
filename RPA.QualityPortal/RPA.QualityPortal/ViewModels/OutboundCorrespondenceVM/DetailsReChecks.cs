using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RPA.QualityPortal.ViewModels
{

    [ExcludeFromCodeCoverage]
    public class DetailsReChecks
    {
        public OutboundCorrespondence OutboundCorrespondence { get; set; }

        public OutboundCorrespondenceReCheck FirstOutboundCorrespondenceReCheck { get; set; }

        public OutboundCorrespondenceReCheck SecondOutboundCorrespondenceReCheck { get; set; }

        public OutboundCorrespondenceReCheck ThirdOutboundCorrespondenceReCheck { get; set; }

        public List<Answer> Answers { get; set; }

        public List<Answer> FirstReCheckAnswers { get; set; }

        public List<Answer> SecondReCheckAnswers { get; set; }

        public List<Answer> ThirdReCheckAnswers { get; set; }

        [Display(Name = "QC Result")]
        public string QCResult { get; set; }

        [Display(Name = "QC ReCheck Result")]
        public string FirstReCheckResult { get; set; }

        [Display(Name = "QC ReCheck Result")]
        public string SecondReCheckResult { get; set; }

        [Display(Name = "QC ReCheck Result")]
        public string ThirdReCheckResult { get; set; }

        public CheckAmendmentReason CheckAmendmentReason { get; set; }

        public CheckAmendmentReason FirstReCheckAmendmentReason { get; set; }

        public CheckAmendmentReason SecondReCheckAmendmentReason { get; set; }

        public CheckAmendmentReason ThirdReCheckAmendmentReason { get; set; }

        public bool Challenge { get; set; }
        public DetailsReChecks()
        {
            Challenge = false;
        }
    }
}