using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using RPA.QualityPortal.Controllers.BankAccount;
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
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class BankAccountReCheckControllerTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        BankAccountReCheckController controller;

        IBankAccountService bankAccountService;
        IPeopleService peopleService;
        IAnswerService answerService;
        Mock<IMessageService> messageService;
        Mock<ISendService> sendService;
        IUserHelper userHelper;
        IRoleManager roleManager;
        ILockService lockService;
        IAuditService auditService;

        [SetUp]
        public void Setup()
        {
            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();

            answerService = new AnswerService(context.MockContext.Object);
            userHelper = new UserHelper(peopleContext.MockContext.Object);
            peopleService = new PeopleService(peopleContext.MockContext.Object);
            sendService = new Mock<ISendService>();
            messageService = new Mock<IMessageService>();
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            auditService = new AuditService(context.MockContext.Object);
            bankAccountService = new BankAccountService(context.MockContext.Object, answerService, lockService);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new BankAccountReCheckController(context.MockContext.Object, peopleContext.MockContext.Object, answerService, messageService.Object, sendService.Object, peopleService, userHelper, roleManager, lockService, bankAccountService, auditService);
        }

        [Test]
        public void Test_Create_Questions_ReCheck_Returns_ReCheckQuestionsView()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.CreateBankReCheckQuestions(53) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CheckAnswers_Returns_Answers()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._CheckAnswer(53, 18) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CheckComments_Returns_Comments()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller._CheckComment(53, 18) as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateBankReCheckQuestions_Redirects_to_CreateBankReCheckResult_if_Valid()
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
                CheckTypeId = 4,
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
            var result = controller.CreateBankReCheckQuestions(53, qaList) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("CreateBankReCheckResult", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateBankReCheckResult_Returns_CreateBankReCheckResult_Page()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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

            controller.Session["CreateBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.CreateBankReCheckResult(53) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateBankReCheckResult_Not_Valid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            //Act
            var result = controller.CreateBankReCheckResult(53) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("GeneralDetails", result.RouteValues["Action"]);      
        }

        [Test]
        public void Test_CreateBankReCheckResult_Save_Not_Valid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            //Bank Account
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 100,
                PersonName = "Gordon, [REDACTED_NAME]",
                BankAccountId = 55,
                QCCompletedByName = "Gordon, Lee George",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
            };

            //Act
            var result = controller.CreateBankReCheckResult(53, bankAccountReCheck, "Approved") as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("GeneralDetails", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateBankReCheckResult_Post_Saves_Pass()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 100,
                PersonName = "Gordon, [REDACTED_NAME]",
                BankAccountId = 54,
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4,
                ReCheckComments = "Test Change \" to '"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
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
                Answer = "No",
                Comment = "Example"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.CreateBankReCheckResult(54, bankAccountReCheck, "Approved") as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("BankAccountDetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateBankReCheckResult_Post_Saves_Fail()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 100,
                PersonName = "Gordon, [REDACTED_NAME]",
                BankAccountId = 53,
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4,
                FailReasonId = 1,
                ReCheckComments = "Not Approved Comment"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
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
                Answer = "No",
                Comment = ""
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["CreateBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.CreateBankReCheckResult(53, bankAccountReCheck, "Not Approved") as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("BankAccountDetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_CreateBankReCheckResult_Post_Fail_Third()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account ReCheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 100,
                PersonName = "Gordon, [REDACTED_NAME]",
                BankAccountId = 56,
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
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

            controller.Session["CreateBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.CreateBankReCheckResult(56, bankAccountReCheck, "Not Approved") as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["Result"].Errors.Any(modelError => modelError.ErrorMessage == "This is the third ReCheck. The QC result must be approved."));
        }

        [Test]
        public void Test_DetailsReCheck_Returns_DetailsReCheck_View()
        {
            var result = controller.BankAccountDetailsReCheck(54) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_DetailsReChecks_Returns_DetailsReChecks_View()
        {
            var result = controller.BankAccountDetailsReChecks(55) as ViewResult;

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// EDIT
        /// </summary>

        [Test]
        public void Test_EditBankReCheckQuestions_Returns_EditBankReCheckQuestions_Page()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.EditBankReCheckQuestions(56) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditBankReCheckQuestions_Redirects_to_EditBankReCheckResult_if_Valid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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
            var result = controller.EditBankReCheckQuestions(56, qaList) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("EditBankReCheckResult", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditBankReCheckQuestions_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //QA List

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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

            controller.Session["EditBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.EditBankReCheckQuestions(56) as ViewResult;

            //Assert
            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);
        }

        [Test]
        public void Test_EditBankReCheckResult_Returns_EditBankReCheckResult_Page()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
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

            controller.Session["EditBankReCheckQuestions"] = qaList;

            //Act

            var result = controller.EditBankReCheckResult(56) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditBankReCheckResult_Redirects_to_AmendmentReason_if_Valid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 56,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
            };

            string result = "Approved";

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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

            controller.Session["EditBankReCheckQuestions"] = qaList;

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            var testResult = controller.EditBankReCheckResult(bankAccountReCheck, result) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("BankAmendmentReasonReCheck", testResult.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditBankReCheckResult_InValid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 60,
                PersonName = "Gordon, Lee",
                QCCompletedByName = "Gordon, Lee",
                ReCheckCompletedBy = "Gordon, Lee",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4,
                BankAccountId = 55
            };

            string result = "Not Approved";

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            Question question = new Question
            {
                QuestionId = 1,
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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

            controller.Session["EditBankReCheckQuestions"] = qaList;

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            var testResult = controller.EditBankReCheckResult(bankAccountReCheck, result) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["Result"].Errors.Any(modelError => modelError.ErrorMessage == "This is the third ReCheck. The QC result must be approved."));
            Assert.IsTrue(controller.ModelState["FailReasonId"].Errors.Any(modelError => modelError.ErrorMessage == "The not approved Reason is required."));
            Assert.IsTrue(controller.ModelState["Comments"].Errors.Any(modelError => modelError.ErrorMessage == "As the result is not approved, the reason(s) must be stated."));
            Assert.IsTrue(controller.ModelState["PersonName"].Errors.Any(modelError => modelError.ErrorMessage == "Please Select a Valid Name From List"));
        }

        [Test]
        public void Test_EditBankReCheckResults_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 56,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
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

            controller.Session["EditBankReCheckQuestions"] = qaList;

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            //Act
            var testResult = controller.EditBankReCheckResult(60) as ViewResult;

            //Assert
            Assert.AreEqual("Gordon, [REDACTED_NAME]", ((BankAccountReCheck)testResult.ViewData.Model).PersonName);
        }

        [Test]
        public void Test_AmendmentReason_Returns_AmendmentReason_Page()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 56,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
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
                Text = "Has the correct type of response been used?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
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

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            controller.Session["EditBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.BankAmendmentReasonReCheck() as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_AmendmentReason_Post_Saves_QC()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 56,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4,
                BankAccountId = 55, 
                ReCheckComments = "Test Change \" to '"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };

            Question question = new Question
            {
                QuestionId = 29,
                Text = "What Colour?"
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 4,
                QuestionId = 29,
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
                CheckId = 56,
                AmendmentReasonId = 1,
                AmendmentReason = amendmentReason,
                AmendmentReasonComments = "Test"
            };

            qaList.QuestionAnswers.Add(QA);

            builder.InitializeController(controller);

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            controller.Session["EditBankReCheckQuestions"] = qaList;

            //Act
            var result = controller.BankAmendmentReasonReCheck(CAR) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("DetailsReCheck", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_AmendmentReason_Post_Invalid()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account Recheck
            BankAccountReCheck bankAccountReCheck = new BankAccountReCheck
            {
                CheckId = 56,
                PersonName = "Gordon, [REDACTED_NAME]",
                QCCompletedByName = "Gordon, [REDACTED_NAME]",
                ReCheckCompletedBy = "Gordon, [REDACTED_NAME]",
                DateQCCompleted = DateTime.Now,
                Archived = false,
                CheckTypeId = 4
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

            controller.Session["EditBankReCheckResult"] = bankAccountReCheck;

            controller.Session["EditBankReCheckQuestions"] = qaList;

            //Act
            controller.BankAmendmentReasonReCheck(CAR);

            //Assert
            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState["AmendmentReason"].Errors.Any(modelError => modelError.ErrorMessage == "An amendment reason must be selected - if no amendments have been made please return to details screen"));
        }
    }
}