using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class CRMRefPrefixData
    {
        public static List<CRMRefPrefix> Data()
        {
            return new List<CRMRefPrefix>
            {
                new CRMRefPrefix
                {
                    CRMRefPrefixId = 1,
                    Text = "CRM -",
                    Active = true
                }
            };
        }
    }
}
