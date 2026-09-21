using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using PagedList;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Factory;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.BankAccountDropDowns;
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
using Assert = NUnit.Framework.Assert;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class BankAccountControllerTest
    {
        MockQualityContext db;
        MockPeopleContext pdb;

        IUserHelper userHelper;
        Mock<IRoleManager> roleManager;
        IFilterService filterService;
        IPeopleService peopleService;
        IAnswerService answerService;
        IBankAccountService bankAccountService;
        IAuditService auditService;
        ILockService lockService;
        IExportService exportService;

        BankAccountController controller;

        [SetUp]
        public void Setup()
        {
            db = new MockQualityContext();
            pdb = new MockPeopleContext();

            userHelper = new UserHelper(pdb.MockContext.Object);
            roleManager = new Mock<IRoleManager>();
            filterService = new FilterService(db.MockContext.Object, pdb.MockContext.Object, userHelper, roleManager.Object);
            peopleService = new PeopleService(pdb.MockContext.Object);
            answerService = new AnswerService(db.MockContext.Object);
            lockService = new LockService(db.MockContext.Object, pdb.MockContext.Object, userHelper);
            bankAccountService = new BankAccountService(db.MockContext.Object, answerService, lockService);
            auditService = new AuditService(db.MockContext.Object);
            exportService = new ExportService(db.MockContext.Object);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new BankAccountController(db.MockContext.Object, pdb.MockContext.Object, userHelper, roleManager.Object, filterService, peopleService, answerService, lockService, bankAccountService, auditService, exportService);
        }

        [Test]
        public void Index()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Complete()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            ViewResult result = controller.Complete() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Outstanding()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            ViewResult result = controller.Outstanding() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Export()
        {
            var result = controller.Export() as FileResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateGeneral()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            ViewResult result = controller.CreateGeneral(53) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateGeneral_Lock()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            RedirectToRouteResult result = controller.CreateGeneral(55) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateGeneral_Post()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();

            // Act
            RedirectToRouteResult result = controller.CreateGeneral(bankAccount) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateGeneral_Post_Lock()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccountLock();

            // Act
            RedirectToRouteResult result = controller.CreateGeneral(bankAccount) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateGeneral_Post_Invalid()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = new BankAccountCheck();

            // Act
            ViewResult result = controller.CreateGeneral(bankAccount) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(!controller.ModelState.IsValid);
        }

        [Test]
        public void CreateQuestions()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();
            controller.Session["CreateGeneral"] = bankAccount;

            // Act
            ViewResult result = controller.CreateQuestions() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateQuestions_No_BankAccountInSession()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            RedirectToRouteResult result = controller.CreateQuestions() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateQuestions_QuestionAnswerListInSession()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();
            controller.Session["CreateGeneral"] = bankAccount;

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            ViewResult result = controller.CreateQuestions() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateQuestions_Post()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();

            BankAccountCheck bankAccount = GetBankAccount();
            controller.Session["CreateGeneral"] = bankAccount;

            // Act
            RedirectToRouteResult result = controller.CreateQuestions(questionAnswerList) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateQuestions_Post_Lock()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();

            BankAccountCheck bankAccount = GetBankAccountLock();
            controller.Session["CreateGeneral"] = bankAccount;

            // Act
            RedirectToRouteResult result = controller.CreateQuestions(questionAnswerList) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();
            controller.Session["CreateGeneral"] = bankAccount;

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            ViewResult result = controller.CreateResult() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult_No_BankAccountInSession()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            RedirectToRouteResult result = controller.CreateResult() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult_No_QuestionAnswerListInSession()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();
            controller.Session["CreateGeneral"] = bankAccount;

            // Act
            RedirectToRouteResult result = controller.CreateResult() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult_Post()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            RedirectToRouteResult result = controller.CreateResult(bankAccount, "Approved") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult_Post_Lock()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccountLock();

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            RedirectToRouteResult result = controller.CreateResult(bankAccount, "Approved") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateResult_Post_Invalid()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();

            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();
            controller.Session["CreateQuestions"] = questionAnswerList;

            // Act
            ViewResult result = controller.CreateResult(bankAccount, "Not Approved") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(!controller.ModelState.IsValid);
        }

        [Test]
        public void CreateResult_Post_No_QuestionAnswerListInSession()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            BankAccountCheck bankAccount = GetBankAccount();

            // Act
            RedirectToRouteResult result = controller.CreateResult(bankAccount, "Not Approved") as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Details()
        {
            // Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            // Act
            ViewResult result = controller.Details(53) as ViewResult;
            BankAccountDetails bankAccountDetails = (BankAccountDetails)result.Model;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(bankAccountDetails.BankAccount);
        }

        private BankAccountCheck GetBankAccount()
        {
            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                Comments = "Test Change \" to '"
            };

            return bankAccount;
        }

        private BankAccountCheck GetBankAccountLock()
        {
            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
                CheckId = 55,
                CheckTypeId = 3,
                CheckType = new CheckType(),
                DateQCCompleted = null,
                DateQCCreated = DateTime.Now,
                EmailNotifications = false,
                FRN = 987654321,
                ManagerName = "Slee, Alan",
                PersonName = "Fazackerley, Paul",
                QCCompletedByName = null,
                SBI = "123456789"
            };

            return bankAccount;
        }

        private QuestionAnswerList GetQuestionAnswerList()
        {
            QuestionAnswerList questionAnswerList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };

            Question question = new Question
            { 
                QuestionId = 1,
                Text = "Who's next?"
            };

            CheckQuestion checkQuestion = new CheckQuestion
            {
                CheckTypeId = 3,
                CheckType = new CheckType(),
                QuestionId = 1,
                Question = question,
                Order = 1
            };

            QuestionAnswer questionAnswer = new QuestionAnswer
            {
                CheckQuestion = checkQuestion,
                Answer = "Yes"
            };

            questionAnswerList.QuestionAnswers.Add(questionAnswer);

            return questionAnswerList;
        }

        [Test]
        public void Test_Date_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index(false, "Dormand, Scott", "01/01/2020", DateTime.Now.AddDays(1).ToString(), 1, 50, null) as ViewResult;

            Assert.AreEqual(4, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_Date_From_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index(true, "Dormand, Scott", DateTime.Now.AddDays(-1).ToString(), null, 1, 50, null) as ViewResult;

            Assert.AreEqual(4, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_Date_To_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Complete("Fazackerley, Paul", null, DateTime.Now.AddDays(1).ToString(), 1, 50, null) as ViewResult;

            Assert.AreEqual(1, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_No_Date_To_Search()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Outstanding("Dormand, Scott", null, null, 1, 50, null) as ViewResult;

            Assert.AreEqual(3, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_Yes_Coaching_Point()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index(false, null, null, null, 1, 50, "coachingPointYes") as ViewResult;

            Assert.AreEqual(1, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_No_Coaching_Point()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            var result = controller.Index(false,  null, null, null, 1, 50, "coachingPointNo") as ViewResult;

            Assert.AreEqual(4, ((PagedList<BankAccountOverview>)result.ViewData.Model).Count);
        }

        [Test]
        public void Test_EditGeneralBank_Post_Redirects_To_EditQuestions_If_Valid()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                SBI = "123456789"
            };

            builder.InitializeController(controller);

            //Act

            var result = controller.EditGeneral(bankAccount) as RedirectToRouteResult;


            //Assert

            Assert.AreEqual("EditQuestions", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditGeneralBank_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                SBI = "123456789"
            };

            builder.InitializeController(controller);

            controller.Session["EditBankGeneral"] = bankAccount;

            //Act

            var result = controller.EditGeneral(53) as ViewResult;

            //Assert

            Assert.AreEqual("Slee, Alan", ((BankAccountCheck)result.ViewData.Model).ManagerName);
        }

        [Test]
        public void Test_EditQuestionsBank_Returns_EditQuestions_Page()
        {
            //Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
                CheckId = 54,
                CheckTypeId = 3,
                CheckType = new CheckType(),
                DateQCCompleted = null,
                DateQCCreated = DateTime.Now,
                EmailNotifications = false,
                FRN = 987654321,
                ManagerName = "Slee, Alan",
                PersonName = "Fazackerley, Paul",
                QCCompletedByName = null,
                SBI = "123456789"
            };

            TestControllerBuilder builder = new TestControllerBuilder();

            builder.InitializeController(controller);

            controller.Session["EditBankGeneral"] = bankAccount;

            var result = controller.EditQuestions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditQuestionsBank_Redirects_to_EditResult_if_Valid()
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
                CheckTypeId = 3,
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
        public void Test_EditQuestionsBank_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            // Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                SBI = "123456789"
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

            controller.Session["EditBankQuestions"] = qaList;

            controller.Session["EditBankGeneral"] = bankAccount;

            //Act

            var result = controller.EditQuestions() as ViewResult;


            //Assert

            Assert.AreEqual("No", ((QuestionAnswerList)result.ViewData.Model).QuestionAnswers[0].Answer);


        }

        [Test]
        public void Test_EditResultBank_Returns_EditResult_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                SBI = "123456789"
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
                CheckTypeId = 3,
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

            controller.Session["EditBankGeneral"] = bankAccount;

            controller.Session["EditBankQuestions"] = qaList;

            //Act

            var result = controller.EditResult() as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_EditResultBank_Post_Saves_QC()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();
            
            //Bank Account
            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                Comments = "Test Change \" to '"
            };
            BankAccountCheck bankAccountExisting = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire Place",
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
                SBI = "123456789"
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
                Text = "What Colour?",
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 3,
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
            BankAccountComments bankAccountComments = new BankAccountComments
            {
                QuestionId = 1,
                AnswerId = 2,
                BankAccountCommentId = 1,
                CheckId = 53,
                AnswerComment = "Test that \" replace by '"
            };

            qaList.QuestionAnswers.Add(QA);
            builder.InitializeController(controller);
            controller.Session["EditBankGeneral"] = bankAccountExisting;
            controller.Session["EditBankQuestions"] = qaList;
            //Act
            var result = controller.EditResult(bankAccount, "Approved") as RedirectToRouteResult;
            //Assert
            Assert.AreEqual("AmendmentReason", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_EditResultBank_Displays_Stored_Session_Data_When_Returned_To()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                FailReason = new FailReason { Active = true, Text = "Test", CheckTypeId = 3, FailReasonId = 1},
                FailReasonId = 1
            };

            builder.InitializeController(controller);

            controller.Session["EditBankGeneral"] = bankAccount;

            //Act

            var result = controller.EditGeneral(2) as ViewResult;

            //Assert

            Assert.AreEqual(1, ((BankAccountCheck)result.ViewData.Model).FailReasonId);
        }

        [Test]
        public void Test_AmendmentReasonBank_Returns_AmendmentReason_Page()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            //Outbound Correspondence

            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire",
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
                SBI = "123456789"
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
                CheckTypeId = 3,
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

            controller.Session["EditBankGeneral"] = bankAccount;

            controller.Session["EditBankQuestions"] = qaList;

            //Act

            var result = controller.AmendmentReason() as ViewResult;

            //Assert

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_AmendmentReasonBank_Post_Saves_QC()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account
            BankAccountCheck bankAccountExisting = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire Place",
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
                Comments = "Test Change \" to '"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved",
            };
            Question question = new Question
            {
                QuestionId = 18,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 3,
                QuestionId = 18,
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

            controller.Session["EditBankGeneral"] = bankAccountExisting;

            controller.Session["EditBankQuestions"] = qaList;

            //Act
            var result = controller.AmendmentReason(CAR) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_AmendmentReasonBank_Post_Adds_Model_Error()
        {
            //Arrange
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account
            BankAccountCheck bankAccountExisting = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire Place",
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
                SBI = "123456789"
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

            controller.Session["EditBankGeneral"] = bankAccountExisting;

            controller.Session["EditBankQuestions"] = qaList;

            //Act
            var result = controller.AmendmentReason(CAR) as RedirectToRouteResult;

            //Assert
            Assert.IsTrue(controller.ModelState["AmendmentReason"].Errors.Any(modelError => modelError.ErrorMessage == "An amendment reason must be selected - if no amendments have been made please return to details screen"));
        }

        [Test]
        public void Test_GeneralDetails_View()
        {
            var result = controller.GeneralDetails(53) as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Challenge_View_is_returned_when_requested()
        {
            //Act
            var result = controller.Challenge(53) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_Overturned_Challenge_with_Recheck_Saves_Correctly()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 1,
                CheckId = 56
            };

            builder.InitializeController(controller);

            //Act
            var result = controller.Challenge("Yes", challenge) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("ChallengeQuestions", result.RouteValues["Action"]);
            Assert.AreEqual(db.MockContext.Object.BankAccountReChecks.Where(x => x.BankAccountId == challenge.CheckId).Count(), 0);
        }

        [Test]
        public void Test_Result_Upheld_Challenge_with_Recheck_Saves_Correctly()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 2,
                CheckId = 56
            };

            builder.InitializeController(controller);

            //Act
            var result = controller.Challenge("Yes", challenge) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
            db.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Overturned_Challenge_without_Recheck_Saves_Correctly()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 1,
                CheckId = 53
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
            TestControllerBuilder builder = new TestControllerBuilder();

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 2,
                CheckId = 53
            };

            builder.InitializeController(controller);

            //Act
            var result = controller.Challenge("No", challenge) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
            db.MockContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void Test_Challenge_Partial_View_is_returned_when_requested()
        {
            //Act
            var result = controller._ChallengeView(53) as PartialViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ChallengeQuestions_returns_ChallengeQuestions()
        {
            //Act
            var result = controller.ChallengeQuestions(53) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ChallengeQuestionsPost_Redirects()
        {
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
                CheckTypeId = 3,
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
            var result = controller.ChallengeQuestions(57, qaList) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("ChallengeResult", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_ChallengeResults_returns_ChallengeResults()
        {
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
                CheckTypeId = 3,
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
            var result = controller.ChallengeResult(53) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_ChallengeResult_Post_ReDirects()
        {
            TestControllerBuilder builder = new TestControllerBuilder();

            //Bank Account
            BankAccountCheck bankAccount = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire Place",
                CheckId = 56,
                CheckTypeId = 3,
                CheckType = new CheckType(),
                DateQCCompleted = null,
                DateQCCreated = DateTime.Now,
                EmailNotifications = false,
                FRN = 987654321,
                ManagerName = "Slee, Alan",
                PersonName = "Fazackerley, Paul",
                QCCompletedByName = null,
                SBI = "123456789"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 18,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 3,
                QuestionId = 18,
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

            controller.Session["ChallengeBankQuestions"] = qaList;
            controller.Session["ChallengeBankGeneral"] = bankAccount;

            //Act
            var result = controller.ChallengeResult(bankAccount, "Approved") as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("ChallengeAmendmentReason", result.RouteValues["Action"]);
        }

        [Test]
        public void Test_Challenge_Amendment_Reason_Returns_View()
        {
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

            //Bank Account
            BankAccountCheck bankAccountExisting = new BankAccountCheck
            {
                Archived = false,
                BACheckTypeId = 1,
                BACheckType = new BACheckType { BACheckTypeId = 1, Text = "Telephone", Active = true },
                BusinessName = "Arcade Fire Place",
                CheckId = 56,
                CheckTypeId = 3,
                CheckType = new CheckType(),
                DateQCCompleted = null,
                DateQCCreated = DateTime.Now,
                EmailNotifications = false,
                FRN = 987654321,
                ManagerName = "Slee, Alan",
                PersonName = "Fazackerley, Paul",
                QCCompletedByName = null,
                SBI = "123456789"
            };

            //QA List
            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>(),
                QCResult = "Approved"
            };
            Question question = new Question
            {
                QuestionId = 18,
                Text = "What Colour?"
            };
            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 3,
                QuestionId = 18,
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

            controller.Session["ChallengeBankGeneral"] = bankAccountExisting;

            controller.Session["ChallengeBankQuestions"] = qaList;

            //Act
            var result = controller.ChallengeAmendmentReason(CAR) as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
            db.MockContext.Verify(x => x.SaveChanges(), Times.AtLeastOnce());
        }
    }
}
