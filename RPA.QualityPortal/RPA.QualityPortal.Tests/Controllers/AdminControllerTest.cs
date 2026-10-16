using Microsoft.AspNetCore.Mvc.ViewComponents;
using RPA.QualityPortal.ViewComponents;
using RPA.QualityPortal.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
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
            userHelper = new UserHelper(peopleContext.MockContext.Object, MockHttpContext.GetMockedHttpContextAccessor());

            answerService = new AnswerService(context.MockContext.Object);
            roleManager = new Mock<IRoleManager>();
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);
            messageService = new EmailService(context.MockContext.Object, peopleContext.MockContext.Object);
            accessService = new AccessService(context.MockContext.Object, peopleContext.MockContext.Object, new ConfigurationBuilder().Build());
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            bankAccountService = new BankAccountService(context.MockContext.Object, answerService, lockService);
            sendService = new SendService(new ConfigurationBuilder().Build());

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

            var result = new TurnOffEmailViewComponent(context.MockContext.Object).WithViewContext().Invoke("OutboundCorrespondence") as ViewViewComponentResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_TurnOffEmail_Returns_TurnOffEmail_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = new TurnOffEmailViewComponent(context.MockContext.Object).WithViewContext().Invoke("Bank") as ViewViewComponentResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ReCheckEmails_Returns_ReCheckEmails_Page_OC()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = new ReCheckEmailsViewComponent(context.MockContext.Object, outboundCorrespondenceService).WithViewContext().Invoke() as ViewViewComponentResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_BankReCheckEmails_Returns_BankReCheckEmails_Page_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = new BankReCheckEmailsViewComponent(context.MockContext.Object, bankAccountService).WithViewContext().Invoke() as ViewViewComponentResult;

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

            var result = controller._TurnOffEmail(control) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
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
                        PersonName = "Gordon, Lee George",
                        QCCompletedByName = "Gordon, Lee George",
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
                        ManagerName = "Gordon, Lee George",
                        HEO = "Gordon, Lee George",
                        SEO = "Gordon, Lee George",
                        EmailNotifications = false
                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._OutboundCorrespondenceEmails(qualityChecks) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
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
                        PersonName = "Gordon, Lee George",
                        QCCompletedByName = "Gordon, Lee George",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 2,
                        CheckType =  new CheckType{ CheckTypeId = 2, Name = "Outbound Correspondence ReCheck" },
                        ReCheckCompletedBy = "Gordon, Lee George",
                        EmailNotifications = false
                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._ReCheckEmails(qualityChecks) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
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
                        PersonName = "Gordon, Lee George",
                        QCCompletedByName = "Gordon, Lee George",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 3,
                        SBI = "123456789",
                        FRN = 123456789,
                        ManagerName = "Gordon, Lee George",
                        EmailNotifications = false,
                        CoachingPoint = false,
                        BusinessName = "Test Business"
                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._BankEmails(qualityChecks) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
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
                        PersonName = "Gordon, Lee George",
                        QCCompletedByName = "Gordon, Lee George",
                        DateQCCompleted = DateTime.Now,
                        Archived = false,
                        CheckTypeId = 4,
                        BankAccountId = 56,
                        EmailNotifications = false,

                    },
                }
            };

            builder.InitializeController(controller);

            var result = controller._BankReCheckEmails(qualityChecks) as RedirectToActionResult;

            Assert.AreEqual("Index", result.ActionName);
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

            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Quality Checks: OC Admin") }, "Test")) } };

            var result = controller.UnlockCheck("OutboundCorrespondence") as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Unlock_Returns_Unlock_Bank()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Quality Checks: Bank Admin") }, "Test")) } };

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
            var result = new OutboundCorrespondenceEmailsViewComponent(context.MockContext.Object, filterService, outboundCorrespondenceService).WithViewContext().Invoke();

            Assert.IsNotNull(result);
        }


        [Test]
        public void Test_Bank_Emails_Returns_Partial_View()
        {
            var result = new BankEmailsViewComponent(context.MockContext.Object, filterService, bankAccountService).WithViewContext().Invoke();

            Assert.IsNotNull(result);
        }
    }
}
