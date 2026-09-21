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
    public class Challenge
    {
        [Key]
        public int ChallengeId { get; set; }

        public int CheckId { get; set; }

        public int ChallengeOutcomeId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Challenge")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime ChallengeDate { get; set; }

        public Challenge()
        {
            ChallengeDate = DateTime.UtcNow;
        }

        public virtual ChallengeOutcome ChallengeOutcome { get; set; }

        public virtual Check Check { get; set; }
    }
}