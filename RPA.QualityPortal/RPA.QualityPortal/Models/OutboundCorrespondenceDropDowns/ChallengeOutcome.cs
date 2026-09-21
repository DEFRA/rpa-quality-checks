using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;

namespace RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns
{
    [ExcludeFromCodeCoverage]
    public class ChallengeOutcome
    {
        public int ChallengeOutcomeId { get; set; }

        [Display(Name = "Challenge Outcome")]
        public string Text { get; set; }

        public bool Active { get; set; }

    }
}