using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Data.Mock
{
    public static class OutboundCorrespondenceData
    {
        public static List<OutboundCorrespondence> Data()
        {
            return new List<OutboundCorrespondence>
            {
                new OutboundCorrespondence
                {
                    CheckId = 1,
                    PersonName = "Gordon, [REDACTED_NAME]",
                    QCCompletedByName = "Gordon, [REDACTED_NAME]",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType(),
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea(),
                    CorrespondenceTypeId = 4,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text = "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason(),
                    SchemeId = 2,
                    Scheme = new Scheme { SchemeId = 2, Text = "Basic Payment Scheme", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "1234567",
                    UniqueId = "1234",
                    SBI = "123456789",
                    CRMRef = "123456",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Gordon, [REDACTED_NAME]",
                    HEO = "Gordon, [REDACTED_NAME]",
                    SEO = "Gordon, [REDACTED_NAME]",
                    Comments = "Test",
                    ReCheckRequired = true,
                    ExcludeQCResult = true,
                    InitialReCheckDecision = true,
                    EmailNotifications = true
                },
                new OutboundCorrespondence
                {
                    CheckId = 2,
                    PersonName = "Dormand, Scott",
                    QCCompletedByName = "Dormand, Scott",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType(),
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea(),
                    CorrespondenceTypeId = 3,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text =  "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason(),
                    SchemeId = 1,
                    Scheme = new Scheme { SchemeId = 1, Text = "Countryside Stewardship", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "7654321",
                    UniqueId = "4321",
                    SBI = "987654321",
                    CRMRef = "654321",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Dormand, Scott",
                    HEO = "Dormand, Scott",
                    SEO = "Dormand, Scott",
                    Comments = "Test 2",
                    ReCheckRequired = false,
                    ExcludeQCResult = false
                },
                new OutboundCorrespondence
                {
                    CheckId = 5,
                    PersonName = "Toward, Fay",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType{ CheckTypeId = 1, Name = "Outbound Correspondence", Order = 1 },
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea{ BusinessAreaId = 1, Text = "Test", Active = true },
                    CorrespondenceTypeId = 3,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text =  "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix{ CRMRefPrefixId = 1, Text = "CRM -", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    SchemeId = 3,
                    Scheme = new Scheme{ SchemeId = 3, Text = "Environmental Stewardship", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "3333333",
                    UniqueId = "3333",
                    SBI = "333333333",
                    CRMRef = "333333",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Toward, Fay",
                    HEO = "Toward, Fay",
                    SEO = "Toward, Fay",
                    Comments = "Test 2",
                    ReCheckRequired = true,
                    ExcludeQCResult = false
                },
                new OutboundCorrespondence
                {
                    CheckId = 6,
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType{ CheckTypeId = 1, Name = "Outbound Correspondence", Order = 1 },
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea{ BusinessAreaId = 1, Text = "Test", Active = true },
                    CorrespondenceTypeId = 3,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text =  "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix{ CRMRefPrefixId = 1, Text = "CRM -", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    SchemeId = 3,
                    Scheme = new Scheme{ SchemeId = 3, Text = "Environmental Stewardship", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "3333333",
                    UniqueId = "3333",
                    SBI = "333333333",
                    CRMRef = "333333",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Slee, Alan",
                    HEO = "Toward, Fay",
                    SEO = "Toward, Fay",
                    Comments = "Test 2",
                    ReCheckRequired = true,
                    ExcludeQCResult = false
                },
                new OutboundCorrespondence
                {
                    CheckId = 7,
                    PersonName = "Fazackerley, Paul",
                    QCCompletedByName = "Toward, Fay",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType{ CheckTypeId = 1, Name = "Outbound Correspondence", Order = 1 },
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea{ BusinessAreaId = 1, Text = "Test", Active = true },
                    CorrespondenceTypeId = 3,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text =  "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix{ CRMRefPrefixId = 1, Text = "CRM -", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason{ FailReasonId = 1, Text = "Test", Active = true },
                    SchemeId = 3,
                    Scheme = new Scheme{ SchemeId = 3, Text = "Environmental Stewardship", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "3333333",
                    UniqueId = "3333",
                    SBI = "333333333",
                    CRMRef = "333333",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Toward, Fay",
                    HEO = "Toward, Fay",
                    SEO = "Toward, Fay",
                    Comments = "Test 2",
                    ReCheckRequired = true,
                    ExcludeQCResult = false
                }
            };
        }
    }
}
