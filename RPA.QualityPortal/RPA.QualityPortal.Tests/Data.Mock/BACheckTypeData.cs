using RPA.QualityPortal.Models.BankAccountDropDowns;
using System.Collections.Generic;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class BACheckTypeData
    {
        public static List<BACheckType> Data()
        {
            return new List<BACheckType>
            {
                new BACheckType
                {
                    BACheckTypeId = 1,
                    Text = "Telephone",
                    Active = true
                }
            };
        }
    }
}
