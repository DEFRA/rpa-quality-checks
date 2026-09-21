using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using PagedList;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Factory;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using RPA.QualityPortal.Tests.Factory;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class OutboundCorrespondenceControllerTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        OutboundCorrespondenceController controller;


        IOutboundCorrespondenceService outboundCorrespondenceService;
        IPeopleService peopleService;
        IAnswerService answerService;
        Mock<IMessageService> messageService;
        IExportService exportService;
        IAuditService auditService;
        Mock<ISendService> sendService;
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
            userHelper = new UserHelper(peopleContext.MockContext.Object);
            roleManager = new Mock<IRoleManager>();
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            sendService = new Mock<ISendService>();
            messageService = new Mock<IMessageService>();
            auditService = new AuditService(context.MockContext.Object);
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            exportService = new ExportService(context.MockContext.Object);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new OutboundCorrespondenceController(context.MockContext.Object, peopleContext.MockContext.Object, outboundCorrespondenceService, peopleService, messageService.Object, sendService.Object, auditService, filterService, answerService, userHelper, exportService, roleManager.Object, lockService);
        }

        //Test here that index page returns all checks
        [Test]
        public void Test_Index_Returns_Index_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Index_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index(true, "Gordon, [REDACTED_NAME]", 1, 50, 0) as ViewResult;

            Assert.AreEqual(1, ((PagedList<OutboundCorrespondenceOverview>)result.ViewData.Model).Count);
        }

        //Test here that complete page returns all complete checks
        [Test]
        public void Test_Complete_Returns_Complete_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Complete() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Complete_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Complete("Dormand, Scott", 1, 50, 0) as ViewResult;

            Assert.AreEqual(1, ((PagedList<OutboundCorrespondenceOverview>)result.ViewData.Model).Count);
        }

        //Test here that outstanding page returns all outstanding checks
        [Test]
        public void Test_Outstanding_Returns_Outstanding_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Outstanding() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Outstanding_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Outstanding("Gordon, [REDACTED_NAME]", 1, 50, 0) as ViewResult;

            Assert.AreEqual(1, ((PagedList<OutboundCorrespondenceOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_CreateGeneral_Returns_CreateGeneral_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.CreateGeneral() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateGeneral_Post_Redirects_To_CreateQuestions_If_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType { CorrespondenceTypeId = 4, Text = "Letter - Bespoke", Active = true },
                CRMRefPrefixId = 1,
                FailReasonId = 1,
                SchemeId = 1,
                UniqueIdentifierPrefixId = 1,
                TemplateReference = "1234567",
                UniqueId = "1235",
                SBI = "123456789",
                CRMRef = "123457",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            builder.InitializeController(controller);

            //Act

            var result = controller.CreateGeneral(outboundCorrespondence) as RedirectToRouteResult;


            //Assert

            Assert.AreEqual("CreateQuestions", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateGeneral_Name_Not_Valid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 3,
                PersonName = "Dormandy, Scott",
                QCCompletedByName = "Dormandy, Scott",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType { CorrespondenceTypeId = 3, Text = "Letter - Template", Active = true },
                CRMRefPrefixId = 1,
                FailReasonId = 1,
                SchemeId = 1,
                UniqueIdentifierPrefixId = 1,
                TemplateReference = "FR1234567",
                UniqueId = "1234",
                SBI = "123456789",
                CRMRef = "123456",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Dormandy, Scott",
                HEO = "Dormandy, Scott",
                SEO = "Dormandy, Scott",
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };
            builder.InitializeController(controller);
            //Act
            controller.CreateGeneral(outboundCorrespondence);
            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["PersonName"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
            Assert.IsTrue(controller.ModelState["ManagerName"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
            Assert.IsTrue(controller.ModelState["SEO"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
            Assert.IsTrue(controller.ModelState["HEO"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
            Assert.IsTrue(controller.ModelState["CRMRef"].Errors.Any(modelError => modelError.ErrorMessage == "A record exists with this CRM Reference and Unique Identififer"));
        }



        [Test]
        public void Test_CreateGeneral_Validation()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 3,
                PersonName = "Dormand, Scott",
                QCCompletedByName = "Dormand, Scott",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType { CorrespondenceTypeId = 4, Text = "Letter - Bespoke", Active = true },
                CRMRefPrefixId = 1,
                FailReasonId = 1,
                UniqueIdentifierPrefixId = 1,
                TemplateReference = "1234567",
                UniqueId = "1234",
                SBI = "123456789",
                CRMRef = "123456",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Dormand, Scott",
                HEO = "Dormand, Scott",
                SEO = "Dormand, Scott",
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };
            builder.InitializeController(controller);
            //Act
            controller.CreateGeneral(outboundCorrespondence);
            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);

        }

        [Test]
        public void Test_CreateGeneral_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.CreateGeneral() as ViewResult;

            //Assert

            Assert.AreEqual("Gordon, [REDACTED_NAME]", ((OutboundCorrespondence)result.ViewData.Model).ManagerName);
        }

        [Test]
        public void Test_CreateQuestions_Returns_CreateQuestions_Page()
        {
            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 2,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 3,
                CRMRefPrefixId = 1,
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondence;

            var result = controller.CreateQuestions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateQuestions_Redirects_to_CreateResult_if_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                CheckType = new CheckType(),
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);


            //Act

            var result = controller.CreateQuestions(qaList) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("CreateResult", result.RouteValues["Action"]);

        }



        [Test]
        public void Test_CreateQuestions_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 2,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 3,
                CRMRefPrefixId = 1,
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };



            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateQuestions"] = qaList;
            controller.Session["CreateGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.CreateQuestions() as ViewResult;


            //Assert

            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);


        }

        [Test]
        public void Test_CreateResult_Returns_CreateResult_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 20,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondence;

            controller.Session["CreateQuestions"] = qaList;

            //Act

            var result = controller.CreateResult() as ViewResult;

            //Assert

            Assert.IsNotNull(result);

        }

        [Test]
        public void Test_CreateResult_Post_Saves_QC()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence


            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
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
                CRMRefPrefix = new CRMRefPrefix(),
                FailReasonId = 1,
                FailReason = new FailReason(),
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


            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
            {
                CheckId = 1,
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
                CRMRefPrefix = new CRMRefPrefix(),
                FailReasonId = 1,
                FailReason = new FailReason(),
                SchemeId = 1,
                Scheme = new Scheme(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
                TemplateReference = "1234567",
                UniqueId = "123454194",
                SBI = "123456789",
                CRMRef = "123456",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]",
                EmailNotifications = true

            };

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 18,
                Text = "What Colour?"

            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 18,
                Question = question,
                Order = 18
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"

            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondenceExisting;

            controller.Session["CreateQuestions"] = qaList;

            //Act

            var result = controller.CreateResult(outboundCorrespondence) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("Details", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_FailReason_Null_Not_Valid()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence


            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
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
                CRMRefPrefix = new CRMRefPrefix(),
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
                ExcludeQCResult = false,
                EmailNotifications = true
            };


            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
            {
                CheckId = 1,
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
                CRMRefPrefix = new CRMRefPrefix(),
                SchemeId = 1,
                Scheme = new Scheme(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
                TemplateReference = "1234567",
                UniqueId = "123454194",
                SBI = "123456789",
                CRMRef = "123456",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]"

            };

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 18,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 18,
                Question = question,
                Order = 18
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"

            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondenceExisting;

            controller.Session["CreateQuestions"] = qaList;

            //Act

            var result = controller.CreateResult(outboundCorrespondence) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["FailReasonId"].Errors.Any(modelError => modelError.ErrorMessage == "A not approved reason must be selected."));
            
        }

        [Test]
        public void Test_CreateResult_Post_Validates()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence


            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
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
                CRMRefPrefix = new CRMRefPrefix(),
                FailReasonId = 1,
                FailReason = new FailReason(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
                TemplateReference = "1234567",
                UniqueId = "1234",
                SBI = "123456789",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };


            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
            {
                CheckId = 1,
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
                CRMRefPrefix = new CRMRefPrefix(),
                FailReasonId = 1,
                FailReason = new FailReason(),
                UniqueIdentifierPrefixId = 1,
                UniqueIdentifierPrefix = new UniqueIdentifierPrefix(),
                TemplateReference = "1234567",
                UniqueId = "1234",
                SBI = "123456789",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]"

            };


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"

            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"

            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateGeneral"] = outboundCorrespondenceExisting;

            controller.Session["CreateQuestions"] = qaList;

            controller.ModelState.AddModelError("CRMRef", "CRMRef Field is Required");

            //Act

            var result = controller.CreateResult(outboundCorrespondence) as RedirectToRouteResult;

            //Assert

            Assert.IsTrue(!controller.ModelState.IsValid);
        }



        //Test here that detail page returns all complete checks
        [Test]
        public void Test_Detail_Returns_Detail_Page()
        {
            var result = controller.Details(2) as ViewResult;

            Assert.IsNotNull(result);
        }

        //Test here that detail page returns all complete checks
        [Test]
        public void Test_Detail_With_ReCheck_Redirects_to_RecheckController()
        {

            var result = controller.Details(1) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditGeneral_Returns_EditGeneral_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.EditGeneral(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditGeneral_Post_Redirects_To_EditQuestions_If_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                BusinessAreaId = 1,
                CorrespondenceTypeId = 1,
                CorrespondenceType = new CorrespondenceType { CorrespondenceTypeId = 4, Text = "Letter - Bespoke", Active = true },
                CRMRefPrefixId = 1,
                FailReasonId = 1,
                SchemeId = 1,
                UniqueIdentifierPrefixId = 1,
                TemplateReference = "1234567",
                UniqueId = "1235",
                SBI = "123456789",
                CRMRef = "123457",
                OutboundCorrespondenceDateSent = DateTime.Now,
                ManagerName = "Gordon, [REDACTED_NAME]",
                HEO = "Gordon, [REDACTED_NAME]",
                SEO = "Gordon, [REDACTED_NAME]",
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            builder.InitializeController(controller);

            //Act

            var result = controller.EditGeneral(outboundCorrespondence) as RedirectToRouteResult;


            //Assert

            Assert.AreEqual("EditQuestions", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditGeneral_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.EditGeneral(2) as ViewResult;

            //Assert

            Assert.AreEqual("Gordon, [REDACTED_NAME]", ((OutboundCorrespondence)result.ViewData.Model).ManagerName);
        }

        [Test]
        public void Test_EditQuestions_Returns_EditQuestions_Page()
        {
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 2,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 1,
                CheckType = new CheckType(),
                BusinessAreaId = 1,
                CorrespondenceTypeId = 1,
                CRMRefPrefixId = 1,
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondence;

            var result = controller.EditQuestions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditQuestions_Redirects_to_EditResult_if_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);


            //Act

            var result = controller.EditQuestions(qaList) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("EditResult", result.RouteValues["Action"]);

        }

        [Test]
        public void Test_EditQuestions_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            // OutboundCorrespondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditQuestions"] = qaList;

            controller.Session["EditGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.EditQuestions() as ViewResult;


            //Assert

            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);


        }

        [Test]
        public void Test_EditResult_Returns_EditResult_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondence;

            controller.Session["EditQuestions"] = qaList;

            //Act

            var result = controller.EditResult() as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditResult_Post_Saves_QC()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            //Outbound Correspondence
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };
            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
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
            };
            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };
            qaList.QuestionAnswers.Add(QA);
            builder.InitializeController(controller);
            controller.Session["EditGeneral"] = outboundCorrespondenceExisting;
            controller.Session["EditQuestions"] = qaList;
            //Act
            var result = controller.EditResult(outboundCorrespondence) as RedirectToRouteResult;
            //Assert
            Assert.AreEqual("AmendmentReason", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditResult_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.EditGeneral(2) as ViewResult;

            //Assert

            Assert.AreEqual(1, ((OutboundCorrespondence)result.ViewData.Model).FailReasonId);
        }

        [Test]
        public void Test_AmendmentReason_Returns_AmendmentReason_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
                FailReasonId = 1,
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
                Comments = "Test",
                ReCheckRequired = true,
                ExcludeQCResult = false
            };


            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondence;

            controller.Session["EditQuestions"] = qaList;

            //Act

            var result = controller.AmendmentReason() as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_AmendmentReason_Post_Saves_QC()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            //Outbound Correspondence
            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
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
            };
            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            AmendmentReason amendmentReason = new AmendmentReason
            {
                AmendmentReasonId = 1,
                Text = "Challenge Outcome",
                Active = true
            };

            CheckAmendmentReason CAR = new CheckAmendmentReason
            {
                CheckId = 1,
                AmendmentReasonId = 1,
                AmendmentReason = amendmentReason,
                AmendmentReasonComments = "Test"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondenceExisting;

            controller.Session["EditQuestions"] = qaList;

            //Act
            var result = controller.AmendmentReason(CAR) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_AmendmentReason_Post_Adds_Model_Error()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            //Outbound Correspondence
            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
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
            };
            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            AmendmentReason amendmentReason = new AmendmentReason
            {
                AmendmentReasonId = 1,
                Text = "Challenge Outcome",
                Active = true
            };

            CheckAmendmentReason CAR = new CheckAmendmentReason
            {
                CheckId = 1,
                AmendmentReasonId = 4,
                AmendmentReason = amendmentReason,
                AmendmentReasonComments = "Test"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditGeneral"] = outboundCorrespondenceExisting;

            controller.Session["EditQuestions"] = qaList;

            //Act
            var result = controller.AmendmentReason(CAR) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(controller.ModelState["AmendmentReason"].Errors.Any(modelError => modelError.ErrorMessage == "An amendment reason must be selected - if no amendments have been made please return to details screen"));
        }



        [Test]

        public void Test_Challenge_View_is_returned_when_requested()
        {
            //Arrange

            //Act

            var result = controller.Challenge(1) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_Overturned_Challenge_with_Recheck_Saves_Correctly()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 1,
                CheckId = 1
            };

            builder.InitializeController(controller);
            //Act

            var result = controller.Challenge("Yes", challenge) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("ChallengeQuestions", result.RouteValues["Action"]);
            Assert.AreEqual(context.MockContext.Object.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == challenge.CheckId).Count(), 0);
        }

        [Test]

        public void Test_Result_Upheld_Challenge_with_Recheck_Saves_Correctly()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 2,
                CheckId = 1
            };

            builder.InitializeController(controller);
            //Act

            var result = controller.Challenge("Yes", challenge) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("Details", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]

        public void Test_Overturned_Challenge_without_Recheck_Saves_Correctly()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 1,
                CheckId = 1
            };

            builder.InitializeController(controller);
            //Act

            var result = controller.Challenge("No", challenge) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("ChallengeQuestions", result.RouteValues["Action"]);

        }

        [Test]

        public void Test_Result_Upheld_Challenge_without_Recheck_Saves_Correctly()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 2,
                CheckId = 1
            };

            builder.InitializeController(controller);
            //Act

            var result = controller.Challenge("No", challenge) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("Details", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]

        public void Test_Challenge_Partial_View_is_returned_when_requested()
        {
            //Arrange

            //Act

            var result = controller._ChallengeView(1) as PartialViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_ChallengeQuestions_returns_ChallengeQuestions()
        {
            //Arrange

            //Act

            var result = controller.ChallengeQuestions(2) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_ChallengeQuestionsPost_Redirects()
        {
            //Arrange

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            //Act

            var result = controller.ChallengeQuestions(2, qaList) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("ChallengeResult", result.RouteValues["Action"]);
        }

        [Test]

        public void Test_ChallengeResults_returns_ChallengeResults()
        {
            //Arrange
            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            controller.Session["ChallengeQuestions"] = qaList;

            //Act

            var result = controller.ChallengeResult(2) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ChallengeResult_Post_ReDirects()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence
            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
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
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };


            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["ChallengeQuestions"] = qaList;
            controller.Session["ChallengeGeneral"] = outboundCorrespondence;

            //Act

            var result = controller.ChallengeResult(outboundCorrespondence) as RedirectToRouteResult;


            //Assert


            Assert.AreEqual("ChallengeAmendmentReason", result.RouteValues["Action"]);

        }

        [Test]

        public void Test_Challenge_Amendment_Reason_Returns_View()
        {
            //Arrange

            //Act

            var result = controller.ChallengeAmendmentReason(2) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_Challenge_Amendment_Reason_Post()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            //Outbound Correspondence
            OutboundCorrespondence outboundCorrespondenceExisting = new OutboundCorrespondence
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
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Question = question,
                Order = 4
            };
            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            AmendmentReason amendmentReason = new AmendmentReason
            {
                AmendmentReasonId = 1,
                Text = "Challenge Outcome",
                Active = true
            };

            CheckAmendmentReason CAR = new CheckAmendmentReason
            {
                CheckId = 1,
                AmendmentReasonId = 1,
                AmendmentReason = amendmentReason,
                AmendmentReasonComments = "Test"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["ChallengeGeneral"] = outboundCorrespondenceExisting;

            controller.Session["ChallengeQuestions"] = qaList;

            //Act

            var result = controller.ChallengeAmendmentReason(CAR) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("Details", result.RouteValues["Action"]);
            context.MockContext.Verify(x => x.SaveChanges(), Times.AtLeastOnce());

        }

        [Test]

        public void Test_ReCheckResults_Returns_ReCheck_Result()
        {
            //Arrange

            //Act

            var result = controller._ReCheckResults(2) as PartialViewResult;


            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_GeneralDetails_View()
        {
            var result = controller.GeneralDetails(2) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Delete_View_Returns_Delete_View_Page()
        {
            var result = controller.DeleteView(2) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_Delete_Deletes_Initial_Check()
        {
            int correspondenceCount = context.MockContext.Object.OutboundCorrespondence.Count();
            var result = controller.Delete(2) as ViewResult;


            context.MockContext.Verify(x => x.SaveChanges(), Times.AtLeastOnce());
            Assert.AreEqual(correspondenceCount - 1, context.MockContext.Object.OutboundCorrespondence.Count());
        }

        [Test]

        public void Test_Delete_Deletes_Initial_Check_and_ReChecks()
        {
            var result = controller.Delete(1) as ViewResult;

            context.MockContext.Verify(x => x.SaveChanges(), Times.AtLeastOnce());
            Assert.AreEqual(0, context.MockContext.Object.OutboundCorrespondenceReCheck.Where(x => x.OutboundCorrespondenceId == 1).Count());
        }

        [Test]
        public void Test_Export()
        {
            var result = controller.Export() as FileResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ExportArchive()
        {
            var result = controller.ExportArchive() as FileResult;

            Assert.IsNotNull(result);
        }
    }
}
