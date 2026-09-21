using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class BankAccountData
    {
        public static List<BankAccountCheck> Data()
        {
            return new List<BankAccountCheck>
            {
                new BankAccountCheck
                {
                    Archived = false,
                    BACheckTypeId = 1,
                    BACheckType = new BACheckType{ BACheckTypeId = 1, Text = "Telephone", Active = true },
                    BusinessName = "[REDACTED_NAME]",
                    CheckId = 53,
                    CheckTypeId = 3,
                    CheckType = new CheckType(),
                    DateQCCompleted = null,
                    DateQCCreated = DateTime.Now,
                    EmailNotifications = false,
                    FRN = 987654321,
                    ManagerName = "Slee, Alan",
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = null,
                    SBI = "123456789",
                    CoachingPoint = true,
                    ReCheckRequired = false,
                },
                new BankAccountCheck
                {
                    Archived = false,
                    BACheckTypeId = 1,
                    BACheckType = new BACheckType{ BACheckTypeId = 1, Text = "Telephone", Active = true },
                    BusinessName = "Arctic Monkeys",
                    CheckId = 54,
                    CheckTypeId = 3,
                    CheckType = new CheckType(),
                    DateQCCompleted = DateTime.Now,
                    DateQCCreated = DateTime.Now,
                    EmailNotifications = true,
                    FRN = 876543219,
                    ManagerName = "Slee, Alan",
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Dormand, Scott",
                    SBI = "234567891",
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    CoachingPoint = false,
                    ReCheckRequired = true,
                    InitialReCheckDecision = true,
                },
                new BankAccountCheck
                {
                    Archived = false,
                    BACheckTypeId = 1,
                    BACheckType = new BACheckType{ BACheckTypeId = 1, Text = "Telephone", Active = true },
                    BusinessName = "Blossoms",
                    CheckId = 55,
                    CheckTypeId = 3,
                    CheckType = new CheckType(),
                    DateQCCompleted = DateTime.Now,
                    DateQCCreated = DateTime.Now,
                    EmailNotifications = false,
                    FRN = 876543219,
                    ManagerName = "Slee, Alan",
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Dormand, Scott",
                    SBI = "345678912",
                    CoachingPoint = false,
                    ReCheckRequired = false,
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true }
                },
                new BankAccountCheck
                {
                    Archived = false,
                    BACheckTypeId = 1,
                    BACheckType = new BACheckType{ BACheckTypeId = 1, Text = "Telephone", Active = true },
                    BusinessName = "Blossoms",
                    CheckId = 56,
                    CheckTypeId = 3,
                    CheckType = new CheckType(),
                    DateQCCompleted = DateTime.Now,
                    DateQCCreated = DateTime.Now,
                    EmailNotifications = false,
                    FRN = 876543219,
                    ManagerName = "Slee, Alan",
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Dormand, Scott",
                    SBI = "345678912",
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    CoachingPoint = false,
                    ReCheckRequired = true,
                },
                new BankAccountCheck
                {
                    Archived = false,
                    BACheckTypeId = 1,
                    BACheckType = new BACheckType{ BACheckTypeId = 1, Text = "Telephone", Active = true },
                    BusinessName = "Blossoms",
                    CheckId = 57,
                    CheckTypeId = 3,
                    CheckType = new CheckType(),
                    DateQCCompleted = DateTime.Now,
                    DateQCCreated = DateTime.Now,
                    EmailNotifications = false,
                    FRN = 876543219,
                    ManagerName = "Dormand, Scott",
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Dormand, Scott",
                    SBI = "345678912",
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    CoachingPoint = false,
                    ReCheckRequired = true,
                }
            };
        }
    }
}
