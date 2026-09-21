using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.DAL;
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
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class OutboundCorrespondenceControllerReCheckTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        OutboundCorrespondenceReCheckController controller;

        IOutboundCorrespondenceService outboundCorrespondenceService;
        IPeopleService peopleService;
        IAnswerService answerService;
        Mock<IMessageService> messageService;
        IAuditService auditService;
        Mock<ISendService> sendService;
        IUserHelper userHelper;
        IRoleManager roleManager;
        IFilterService filterService;
        ILockService lockService;

        [SetUp]
        public void Setup()
        {

            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();


            answerService = new AnswerService(context.MockContext.Object);
            userHelper = new UserHelper(peopleContext.MockContext.Object);
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            sendService = new Mock<ISendService>();
            messageService = new Mock<IMessageService>();
            auditService = new AuditService(context.MockContext.Object);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new OutboundCorrespondenceReCheckController(context.MockContext.Object, peopleContext.MockContext.Object, outboundCorrespondenceService, messageService.Object, sendService.Object, auditService, peopleService, userHelper, roleManager, filterService, lockService);

        }

        [Test]
        public void Test_Recheck_Locked_Return_Index_From_Details()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = (RedirectToRouteResult)controller.CreateReCheckQuestions(2);

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("OutboundCorrespondence", result.RouteValues["controller"]);
        }

        [Test]
        public void Test_Recheck_Locked_Return_Index_From_CreateReCheckQuestions()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Not Approved"
            };

            var result = (RedirectToRouteResult)controller.CreateReCheckQuestions(2, qaList);

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("OutboundCorrespondence", result.RouteValues["controller"]);
        }

        [Test]
        public void Test_Recheck_Locked_Return_Index_From_CreateReCheckResult()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            //Outbound Correspondence

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 2,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2,
                EmailNotifications = true
            };

            var result = (RedirectToRouteResult)controller.CreateReCheckResult(2, outboundCorrespondenceReCheck);

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("OutboundCorrespondence", result.RouteValues["controller"]);
        }

        [Test]
        public void Test_Create_Questions_ReCheck_Returns_ReCheckQuestionsView()
        {


            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.CreateReCheckQuestions(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CheckAnswers_Returns_Answers()
        {

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);


            var result = controller._CheckAnswer(1, 1) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Create_Questions_ReCheck_Disables_Q2_if_disabled_in_main_check()
        {

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);


            var result = controller.CreateReCheckQuestions(5) as ViewResult;

            Assert.AreEqual("N/A", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[1].Answer);
        }

        [Test]
        public void Test_CreateReCheckQuestions_Redirects_to_CreateReCheckResult_if_Valid()
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

            var result = controller.CreateReCheckQuestions(1, qaList) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("CreateReCheckResult", result.RouteValues["Action"]);
        }


        [Test]
        public void Test_CreateReCheckQuestions_Fails_If_Third_ReCheck_Fails()
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

            var result = controller.CreateReCheckQuestions(4, qaList) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["QCResult"].Errors.Any(modelError => modelError.ErrorMessage == "This is the third ReCheck. The QC result must be approved."));

        }

        [Test]
        public void Test_CreateQuestionsReCheck_Displays_Stored_Data_When_Clicking_Back()
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

            controller.Session["CreateReCheckQuestions"] = qaList;

            //Act

            var result = controller.CreateReCheckQuestions(1) as ViewResult;


            //Assert

            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);

        }

        [Test]
        public void Test_CreateReCheckResult_Returns_CreateResult_Page()
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


            controller.Session["CreateReCheckQuestions"] = qaList;

            //Act

            var result = controller.CreateReCheckResult(1) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateReCheckResult_Post_Saves_QC_Fail()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 1,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2,
                EmailNotifications = true,
                OutboundCorrespondenceId = 2
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


            controller.Session["CreateReCheckQuestions"] = qaList;

            //Act

            var result = controller.CreateReCheckResult(1, outboundCorrespondenceReCheck) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateReCheckResult_Post_Saves_QC_Pass()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 2,
                PersonName = "Gordon, [REDACTED_NAME]",
                OutboundCorrespondenceId = 1,
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2

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


            controller.Session["CreateReCheckQuestions"] = qaList;

            //Act

            var result = controller.CreateReCheckResult(1, outboundCorrespondenceReCheck) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateReCheckResult_Post_Saves_QC_Pass_Ad()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 1,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2,
                OutboundCorrespondenceId = 2
            };

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved Advisory"
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


            controller.Session["CreateReCheckQuestions"] = qaList;

            //Act

            var result = controller.CreateReCheckResult(1, outboundCorrespondenceReCheck) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]

        public void Test_DetailsReCheck_Returns_details_ReCheck_View()
        {
            var result = controller.DetailsReCheck(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]

        public void Test_DetailsReChecks_Returns_DetailsReChecks_View()
        {
            var result = controller.DetailsReChecks(5) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditReCheckQuestions_Returns_EditReCheckQuestions_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.EditReCheckQuestions(2) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditReCheckQuestions_Redirects_to_EditReCheckResult_if_Valid()
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
                CheckTypeId = 2,
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

            var result = controller.EditReCheckQuestions(1, qaList) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("EditReCheckResult", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditReCheckQuestions_Displays_Stored_Session_Data_When_Returned_To()
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
                CheckTypeId = 2,
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

            controller.Session["EditReCheckQuestions"] = qaList;

            //Act

            var result = controller.EditReCheckQuestions(1) as ViewResult;


            //Assert

            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);

        }

        [Test]
        public void Test_EditReCheckResult_Returns_EditReCheckResult_Page()
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
                CheckTypeId = 2,
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

            controller.Session["EditReCheckQuestions"] = qaList;

            //Act

            var result = controller.EditReCheckResult(1) as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditReCheckResult_Redirects_to_AmendmentReason_if_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence Re Check

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2

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
                CheckTypeId = 2,
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

            controller.Session["EditReCheckQuestions"] = qaList;

            controller.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            var result = controller.EditReCheckResult(outboundCorrespondenceReCheck) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("AmendmentReasonReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditReCheckResults_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence Re Check

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2
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
                CheckTypeId = 2,
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

            controller.Session["EditReCheckQuestions"] = qaList;

            controller.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            //Act

            var result = controller.EditReCheckResult(3) as ViewResult;

            //Assert

            Assert.AreEqual("Gordon, [REDACTED_NAME]", ((OutboundCorrespondenceReCheck)result.ViewData.Model).PersonName);
        }

        [Test]
        public void Test_AmendmentReason_Returns_AmendmentReason_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence Re Check

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2
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

            controller.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            controller.Session["EditReCheckQuestions"] = qaList;

            //Act

            var result = controller.AmendmentReasonReCheck() as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_AmendmentReason_Post_Saves_QC()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence Re Check

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2,
                FurtherReCheckRequired = false,
                OutboundCorrespondenceId = 1
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

            controller.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            controller.Session["EditReCheckQuestions"] = qaList;

            //Act

            var result = controller.AmendmentReasonReCheck(CAR) as RedirectToRouteResult;

            //Assert

            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

       [Test]
        public void Test_AmendmentReason_Post_Invalid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence Re Check

            OutboundCorrespondenceReCheck outboundCorrespondenceReCheck = new OutboundCorrespondenceReCheck
            {
                CheckId = 3,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 2
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
                CheckId = 1
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditReCheckResult"] = outboundCorrespondenceReCheck;

            controller.Session["EditReCheckQuestions"] = qaList;

            //Act

            controller.AmendmentReasonReCheck(CAR);

            //Assert

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["AmendmentReason"].Errors.Any(modelError => modelError.ErrorMessage == "An amendment reason must be selected - if no amendments have been made please return to details screen"));
        }

        [Test]
        public void Test_DeleteReCheck_View_Returns_DeleteReCheck_View_Page()
        {
            var result = controller.DeleteReCheckView(1) as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
