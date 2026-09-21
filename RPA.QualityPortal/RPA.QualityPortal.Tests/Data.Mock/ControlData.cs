using RPA.QualityPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class ControlData
    {
        public static List<Control> Data()
        {
            return new List<Control>
            {
                new Control
                {
                    ControlId = 1,
                    Property = "OutboundCorrespondenceEmail",
                    Active = true
                },
                new Control
                {
                    ControlId = 1,
                    Property = "BankEmail",
                    Active = true
                },
            };
        }
    }
}

