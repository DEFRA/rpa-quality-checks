using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class SchemeData
    {
        public static List<Scheme> Data()
        {
            return new List<Scheme>
            {
                new Scheme
                {
                    SchemeId = 1,
                    Text = "Countryside Stewardship",
                    Active = true
                },
                new Scheme
                {
                    SchemeId = 2,
                    Text = "Basic Payment Scheme",
                    Active = true
                },
                new Scheme
                {
                    SchemeId = 3,
                    Text = "Environmental Stewardship",
                    Active = true
                },
            };
        }
    }
}
