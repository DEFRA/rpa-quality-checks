using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class CheckResultData
    {
        public static List<CheckResult> Data()
        {
            return new List<CheckResult>
            {
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 1,
                    Result = new Result{ ResultId = 1, Text = "Approved" },
                    ResultId = 1
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 2,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 3,
                    Result = new Result{ ResultId = 2, Text = "Approved Advisory" },
                    ResultId = 2
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 4,
                    Result = new Result{ ResultId = 2, Text = "Approved Advisory" },
                    ResultId = 2
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 5,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 6,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 7,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 8,
                    Result = new Result{ ResultId = 1, Text = "Approved" },
                    ResultId = 1
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 53,
                    Result = new Result{ ResultId = 5, Text = "Approved" },
                    ResultId = 1
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 54,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 55,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 56,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 58,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                },
                new CheckResult
                {
                    Check = new Check(),
                    CheckId = 59,
                    Result = new Result{ ResultId = 3, Text = "Not Approved" },
                    ResultId = 3
                }
            };
        }
    }
}
