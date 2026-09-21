using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class CorrespondenceTypeData
    {
        public static List<CorrespondenceType> Data()
        {
            return new List<CorrespondenceType>
            {
                new CorrespondenceType
                {
                    CorrespondenceTypeId = 1,
                    Text = "Email - Template",
                    Active = true
                },
                new CorrespondenceType
                {
                    CorrespondenceTypeId = 2,
                    Text = "Email - Bespoke",
                    Active = true
                },
                new CorrespondenceType
                {
                    CorrespondenceTypeId = 3,
                    Text = "Letter - Template",
                    Active = true
                },
                new CorrespondenceType
                {
                    CorrespondenceTypeId = 4,
                    Text = "Letter - Bespoke",
                    Active = true
                }
            };
        }
    }
}
