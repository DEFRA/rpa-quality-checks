using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class CheckAmendmentReasonData
    {
        public static List<CheckAmendmentReason> Data()
        {
            return new List<CheckAmendmentReason>
            {
                new CheckAmendmentReason
                {
                    CheckId = 2,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 7,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 8,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 6,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 55,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 58,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 59,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                },
                new CheckAmendmentReason
                {
                    CheckId = 60,
                    AmendmentReasonId = 1,
                    AmendmentReasonComments = "Test",
                    AmendmentReason = new AmendmentReason{ AmendmentReasonId = 1, Text = "Challenge Outcome", Active = true }
                }
            };
        }
    }
}
