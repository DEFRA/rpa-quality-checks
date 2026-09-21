using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class BankAccountReCheckData
    {
        public static List<BankAccountReCheck> Data()
        {
            return new List<BankAccountReCheck>
            {
                new BankAccountReCheck
                {
                    CheckId = 57,
                    BankAccountId = 54,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    ReCheckActive = false,
                    EmailNotifications = true
                },
                new BankAccountReCheck
                {
                    CheckId = 58,
                    BankAccountId = 55,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = false,
                    FurtherReCheckRequired = true,
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true }
                },
                new BankAccountReCheck
                {
                    CheckId = 59,
                    BankAccountId = 55,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    ReCheckActive = false,
                    EmailNotifications = true,
                    FurtherReCheckRequired = true,
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true }
                },
                new BankAccountReCheck
                {
                    CheckId = 60,
                    BankAccountId = 55,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    ReCheckActive = false,
                    EmailNotifications = true
                },
                new BankAccountReCheck
                {
                    CheckId = 61,
                    BankAccountId = 56,
                    ReCheckActive = true,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = false,                 
                },
                new BankAccountReCheck
                {
                    CheckId = 62,
                    BankAccountId = 56,
                    ReCheckActive = true,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 4,
                    CheckType = new CheckType{ CheckTypeId = 4, Name = "Bank Account ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = false,
                }
            };
        }
    }
}

