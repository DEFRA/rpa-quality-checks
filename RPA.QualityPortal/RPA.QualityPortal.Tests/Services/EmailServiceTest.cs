using Moq;
using NUnit.Framework;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System;

namespace RPA.QualityPortal.Tests.Services
{

    [TestFixture]
    public class EmailServiceTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
      
        IOutboundCorrespondenceService outboundCorrespondenceService;
        IPeopleService peopleService;
        IAnswerService answerService;
        IMessageService messageService;
        IFilterService filterService;
        IUserHelper userHelper;
        Mock<IRoleManager> roleManager;
        ILockService lockService;

        [SetUp]
        public void Setup()
        {

            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();
            answerService = new AnswerService(context.MockContext.Object);
            roleManager = new Mock<IRoleManager>();
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            messageService = new EmailService(context.MockContext.Object, peopleContext.MockContext.Object);
        }

        [Test]

        public void Test_Email_Service_Creates_OutboundCorrespondence_Email()
        {
            //Arrange

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 6,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                BusinessArea = new BusinessArea(),
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType(),
                CRMRefPrefixId = 1,
                CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                FailReasonId = 1,
                SchemeId = 1,
                Scheme = new Scheme(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
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
                ExcludeQCResult = false
            };

            //Act

            var result = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondence);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");

        }

        [Test]

        public void Test_Email_Service_Creates_OutboundCorrespondence_ReCheck_Email()
        {
            //Arrange

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 6,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                BusinessArea = new BusinessArea(),
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType(),
                CRMRefPrefixId = 1,
                CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                FailReasonId = 1,
                SchemeId = 1,
                Scheme = new Scheme(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
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
                ExcludeQCResult = false
            };

            //Act

            var result = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondence);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");
        }

        [Test]

        public void Test_Email_Service_Creates_OutboundCorrespondence_No_ReCheck()
        {
            //Arrange

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 6,
                PersonName = "Dormand, Scott",
                QCCompletedByName = "Dormand, Scott",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                BusinessArea = new BusinessArea { BusinessAreaId = 1, Text = "Test", Active = true },
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType { CorrespondenceTypeId = 1, Text = "Test", Active = true },
                CRMRefPrefixId = 1,
                CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                FailReasonId = 1,
                SchemeId = 1,
                Scheme = new Scheme { SchemeId = 1, Text = "Test", Active = true },
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix { UniqueIdentifierPrefixId = 1, Text = "Test", Active = true },
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
                ExcludeQCResult = false
            };

            //Act

            var result = messageService.GenerateOutboundCorrespondenceEmail(outboundCorrespondence);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");

        }

        [Test]

        public void Test_Email_Service_Creates_BankAccount_Email()
        {
            //Arrange

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                CheckId = 53,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 3,
                BACheckTypeId = 1,
                BusinessName = "Test Farm",
                FailReasonId = 1,
                DateQCCreated = DateTime.Now,
                SBI = "123456789",
                FRN = 1234567890,
                CoachingPoint = false,
                ManagerName = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true
            };

            //Act

             var result = messageService.GenerateBankEmail(bankAccount);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");

        }

        [Test]

        public void Test_Email_Service_Creates_BankAccount_Email_with_FailReason()
        {
            //Arrange

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                CheckId = 54,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 3,
                BACheckTypeId = 1,
                BusinessName = "Test Farm",
                FailReasonId = 1,
                DateQCCreated = DateTime.Now,
                SBI = "123456789",
                FRN = 1234567890,
                CoachingPoint = false,
                ManagerName = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true
            };

            //Act

            var result = messageService.GenerateBankEmail(bankAccount);

            //Assert

            Assert.AreEqual(result.CC[0], "[REDACTED_EMAIL]");

        }

        [Test]

        public void Test_Email_Service_Creates_BankAccount_ReCheck_Email()
        {
            //Arrange

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                CheckId = 54,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 3,
                BACheckTypeId = 1,
                BusinessName = "Test Farm",
                FailReasonId = 1,
                DateQCCreated = DateTime.Now,
                SBI = "123456789",
                FRN = 1234567890,
                CoachingPoint = false,
                ManagerName = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true
            };

            //Act

            var result = messageService.GenerateBankEmail(bankAccount);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");

        }

        [Test]

        public void Test_Email_Service_Creates_BankAccount_No_ReCheck()
        {
            //Arrange

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                CheckId = 53,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 3,
                BACheckTypeId = 1,
                BusinessName = "Test Farm",
                FailReasonId = 1,
                DateQCCreated = DateTime.Now,
                SBI = "123456789",
                FRN = 1234567890,
                CoachingPoint = false,
                ManagerName = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = false
            };

            //Act

            var result = messageService.GenerateBankEmail(bankAccount);

            //Assert

            Assert.AreEqual(result.To[0], "[REDACTED_EMAIL]");
        }

        [Test]
        public void Test_Email_Helper_Throws_Exception_TeamMemberNotFound()
        {
            var peopleDb = peopleContext.MockContext.Object;

            Assert.Throws<ArgumentException>(() => EmailHelper.GetOutgoingEmailAddresses(peopleDb, "Blows, Nick", "Fazackerley, Paul"));
        }

        [Test]
        public void Test_Email_Helper_Throws_Exception_LineManagerNotFound()
        {
            var peopleDb = peopleContext.MockContext.Object;

            Assert.Throws<ArgumentException>(() => EmailHelper.GetOutgoingEmailAddresses(peopleDb, "Fazackerley, Paul", "Blows, Nick"));
        }
    }
}