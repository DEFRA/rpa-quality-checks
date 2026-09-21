using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class UniqueIdentifierPrefixData
    {
        public static List<UniqueIdentifierPrefix> Data()
        {
            return new List<UniqueIdentifierPrefix>
            {
                new UniqueIdentifierPrefix
                {
                    UniqueIdentifierPrefixId = 1,
                    Text = "CNF -",
                    Active = true
                }
            };
        }
    }
}
