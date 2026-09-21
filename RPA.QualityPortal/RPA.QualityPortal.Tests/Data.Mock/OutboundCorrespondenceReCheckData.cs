using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class OutboundCorrespondenceReCheckData
    {
        public static List<OutboundCorrespondenceReCheck> Data()
        {
            return new List<OutboundCorrespondenceReCheck>
            {
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 1,
                    OutboundCorrespondenceId = 1,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = false
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 3,
                    OutboundCorrespondenceId = 3,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType = new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    ReCheckActive = false,
                    EmailNotifications = true
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 4,
                    OutboundCorrespondenceId = 4,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = true
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 6,
                    OutboundCorrespondenceId = 5,
                    PersonName = "Toward, Fay",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Toward, Fay",
                    ReCheckActive = false,
                    FurtherReCheckRequired = true,
                    ReCheckComments = "Test",
                    EmailNotifications = true
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 7,
                    OutboundCorrespondenceId = 5,
                    PersonName = "Toward, Fay",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType = new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Toward, Fay",
                    ReCheckActive = false,
                    FurtherReCheckRequired = true,
                    ReCheckComments = "Test",
                    EmailNotifications = true
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 8,
                    OutboundCorrespondenceId = 5,
                    PersonName = "Toward, Fay",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Toward, Fay",
                    FurtherReCheckRequired = false,
                    ReCheckComments = "Test",
                    EmailNotifications = true
                },
                new OutboundCorrespondenceReCheck
                {
                    CheckId = 9,
                    OutboundCorrespondenceId = 4,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                    ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                    EmailNotifications = true
                }
            };
        }
    }
}
