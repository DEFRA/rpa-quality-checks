using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.QualityPortal.Models;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class LockCheckData
    {
        public static List<LockCheck> Data()
        {
            return new List<LockCheck>
            {
                new LockCheck
                {
                    CheckId = 2,
                    LockCheckId = 1,
                    CheckLockedBy = "Example, Test",
                    Timestamp = DateTime.Now.AddHours(-1)
                },
                new LockCheck
                {
                    CheckId = 55,
                    LockCheckId = 2,
                    CheckLockedBy = "Example, Test",
                    Timestamp = DateTime.Now.AddHours(-1)
                }
            };
        }
    }
}
