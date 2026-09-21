using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class AmendmentReasonData
    {
        public static List<AmendmentReason> Data()
        {
            return new List<AmendmentReason>
            {
                new AmendmentReason
                {
                    AmendmentReasonId = 1,
                    Text = "Challenge Outcome",
                    Active = true
                },
                new AmendmentReason
                {
                    AmendmentReasonId = 2,
                    Text = "[REDACTED_NAME]",
                    Active = true
                },
                new AmendmentReason
                {
                    AmendmentReasonId = 3,
                    Text = "Consistency Error",
                    Active = true
                }
            };
        }
    }
}
