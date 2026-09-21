using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class ChallengeData
    {
        public static List<Challenge> Data()
        {
            return new List<Challenge>
            {
                new Challenge
                {
                    ChallengeId =1,
                    ChallengeDate = DateTime.Now,
                    CheckId = 2,
                    ChallengeOutcomeId = 1,
                    ChallengeOutcome = new ChallengeOutcome{ ChallengeOutcomeId = 1, Text = "Original Result Overturned", Active = true }
                }
            };
        }
    }
}
