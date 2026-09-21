using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Factory;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using RPA.QualityPortal.Tests.Factory;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    class AdminControllerTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        AdminController controller;
        
        IOutboundCorrespondenceService outboundCorrespondenceService;
        IAnswerService answerService;
        IMessageService messageService;
        IFilterService filterService;
        IUserHelper userHelper;
        Mock<IRoleManager> roleManager;
        IAccessService accessService;
        IPeopleService peopleService;
        ILockService lockService;
        IBankAccountService bankAccountService;
        ISendService sendService;
        
        [SetUp]
        public void Setup()
        {
            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();
            userHelper = new UserHelper(peopleContext.MockContext.Object);

            answerService = new AnswerService(context.MockContext.Object);
            roleManager = new Mock<IRoleManager>();
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);
            messageService = new EmailService(context.MockContext.Object, peopleContext.MockContext.Object);
            accessService = new AccessService(context.MockContext.Object, peopleContext.MockContext.Object);
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            bankAccountService = new BankAccountService(context.MockContext.Object, answerService, lockService);
            sendService = new SendService();

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new AdminController(context.MockContext.Object, peopleContext.MockContext.Object, outboundCorrespondenceService, bankAccountService, answerService, filterService, messageService, roleManager.Object, userHelper, accessService, peopleService, lockService, sendService);
        }

        [Test]
        public void Test_Index_Returns_Index_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index("OutboundCorrespondence") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Index_Returns_Index_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index("Bank") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Email_Returns_Email_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Email("OutboundCorrespondence") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Email_Returns_Email_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Email("Bank") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_TurnOffEmail_Returns_TurnOffEmail_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._TurnOffEmail("OutboundCorrespondence") as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_TurnOffEmail_Returns_TurnOffEmail_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._TurnOffEmail("Bank") as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ReCheckEmails_Returns_ReCheckEmails_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._ReCheckEmails() as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_BankReCheckEmails_Returns_BankReCheckEmails_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._BankReCheckEmails() as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_TurnOffEmail_Saves()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            Control control = new Control
            {
                ControlId = 2,
                Property = "Email",
                Active = false
            };

            builder.InitializeController(controller);

            var result = controller._TurnOffEmail(control) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Email_Saves_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            List<OutboundCorrespondenceOverview> qualityChecks = new List<OutboundCorrespondenceOverview>
            {
                new OutboundCorrespondenceOverview
                {
                    OutboundCorrespondence = new OutboundCorrespondence
                    {
                        CheckId = 2,
                        PersonName = "Gordon, [REDACTED_NAME]",
                        QCCompletedByName = "Gordon, [REDACTED_NAME]",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 1,
                        BusinessAreaId = 1,
                        CorrespondenceTypeId = 1,
                        CRMRefPrefixId = 1,
                        SchemeId = 1,
                        UniqueIdentifierPrefixId = 1,
                        TemplateReference = "1234567",
                        UniqueId = "1234",
                        SBI = "123456789",
                        CRMRef = "123456",
                        OutboundCorrespondenceDateSent = DateTime.Now,
                        ManagerName = "Gordon, [REDACTED_NAME]",
                        HEO = "Gordon, [REDACTED_NAME]",
                        SEO = "Gordon, [REDACTED_NAME]",
                        EmailNotifications = false
                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._OutboundCorrespondenceEmails(qualityChecks) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_ReCheckEmail_Saves_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            List<OutboundCorrespondenceOverview> qualityChecks = new List<OutboundCorrespondenceOverview>
            {
                new OutboundCorrespondenceOverview
                {
                    OutboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
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
                }
            };

            builder.InitializeController(controller);

            var result = controller._ReCheckEmails(qualityChecks) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }


        [Test]
        public void Test_Email_Saves_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            List<BankAccountOverview> qualityChecks = new List<BankAccountOverview>
            {
                new BankAccountOverview
                {
                 BankAccount = new BankAccountCheck
                    {
                        CheckId = 53,
                        PersonName = "Gordon, [REDACTED_NAME]",
                        QCCompletedByName = "Gordon, [REDACTED_NAME]",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 3,
                        SBI = "123456789",
                        FRN = 123456789,
                        ManagerName = "Gordon, [REDACTED_NAME]",
                        EmailNotifications = false,
                        CoachingPoint = false,
                        BusinessName = "Test Business"
                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._BankEmails(qualityChecks) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_ReCheckEmail_Saves_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            List<BankAccountOverview> qualityChecks = new List<BankAccountOverview>
            {
                new BankAccountOverview
                {
                    BankAccountReCheck = new BankAccountReCheck
                    {
                        CheckId = 60,
                        PersonName = "Gordon, [REDACTED_NAME]",
                        QCCompletedByName = "Gordon, [REDACTED_NAME]",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 4,
                        BankAccountId = 56,
                        EmailNotifications = false,

                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._BankReCheckEmails(qualityChecks) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Access_Returns_Access_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Access("OutboundCorrespondence") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Access_Returns_Access_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Access("Bank") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Delete_Lock_OC()
        {
            controller.DeleteLock(1, "OutboundCorrespondence");

            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Delete_Lock_Bank()
        {
            controller.DeleteLock(2, "Bank");

            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Unlock_Returns_Unlock_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("Quality Checks: OC Admin")).Returns(true);
            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);
            controller.ControllerContext = controllerContextMock.Object;

            var result = controller.UnlockCheck("OutboundCorrespondence") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Unlock_Returns_Unlock_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("Quality Checks: Bank Admin")).Returns(true);
            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);
            controller.ControllerContext = controllerContextMock.Object;

            var result = controller.UnlockCheck("Bank") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Name_Access_Invalid()
        {
            controller.Access("Test", "Test");

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["Name"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
        }

        [Test]
        public void Test_Outbound_Correspondence_Emails_Returns_Partial_View()
        {
            var result = controller._OutboundCorrespondenceEmails();

            Assert.IsNotNull(result);
        }


        [Test]
        public void Test_Bank_Emails_Returns_Partial_View()
        {
            var result = controller._BankEmails();

            Assert.IsNotNull(result);
        }
    }
}
