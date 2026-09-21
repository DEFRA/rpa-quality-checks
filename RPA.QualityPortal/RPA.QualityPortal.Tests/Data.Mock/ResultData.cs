using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class ResultData
    {
        public static List<Result> Data()
        {
            return new List<Result>
            {
                new Result
                {
                    ResultId = 1,
                    Text = "Approved"
                },
                new Result
                {
                    ResultId = 2,
                    Text = "Approved Advisory"
                },
                new Result
                {
                    ResultId = 3,
                    Text = "Not Approved"
                }
            };
        }
    }
}
