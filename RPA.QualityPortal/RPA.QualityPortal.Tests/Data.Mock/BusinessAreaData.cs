using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class BusinessAreaData
    {
        public static List<BusinessArea> Data()
        {
            return new List<BusinessArea>
            {
                new BusinessArea
                {
                    BusinessAreaId = 1,
                    Text = "Test",
                    Active = true
                }
            };
        }
    }
}
