using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class ChallengeOutcomeData
    {
        public static List<ChallengeOutcome> Data()
        {
            return new List<ChallengeOutcome>
            {
                new ChallengeOutcome
                {
                    ChallengeOutcomeId = 1,
                    Text = "Original Result Overturned",
                    Active = true
                },
                new ChallengeOutcome
                {
                    ChallengeOutcomeId = 2,
                    Text = "Original Result Upheld",
                    Active = true
                }
            };
        }
    }
}
