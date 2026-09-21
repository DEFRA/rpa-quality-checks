using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class FailReasonData
    {
        public static List<FailReason> Data()
        {
            return new List<FailReason>
            {
                new FailReason
                {
                    FailReasonId = 1,
                    Text = "Test",
                    Active = true,
                    CheckTypeId = 1,
                    CheckType = new CheckType {CheckTypeId = 1, Name = "Outbound Correspondence", Order = 1 }
                }
            };
        }
    }
}
