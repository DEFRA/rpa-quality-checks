using RPA.QualityPortal.Models;
using System.Collections.Generic;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class CheckTypeData
    {
        public static List<CheckType> Data()
        {
            return new List<CheckType>
            {
                new CheckType
                {
                    CheckTypeId = 1,
                    Name = "Outbound Correspondence",
                    Order = 1

                },
                new CheckType
                {
                    CheckTypeId = 2,
                    Name = "Outbound Correspondence ReCheck",
                    Order = 2
                },
                new CheckType
                {
                    CheckTypeId = 3,
                    Name = "Bank Account",
                    Order = 1
                },
                new CheckType
                {
                    CheckTypeId = 4,
                    Name = "Bank Account ReCheck",
                    Order = 2
                }
            };
        }
    }
}
