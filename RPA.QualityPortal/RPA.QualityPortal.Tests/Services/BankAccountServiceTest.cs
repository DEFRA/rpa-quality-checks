using Moq;
using NUnit.Framework;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.BankAccountDropDowns;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class BankAccountServiceTest
    {
        MockQualityContext db;
        MockPeopleContext pdb;

        IUserHelper userHelper;
        IAnswerService answerService;
        ILockService lockService;

        BankAccountService bankAccountService;

        [SetUp]
        public void Setup()
        {
            db = new MockQualityContext();
            pdb = new MockPeopleContext();

            userHelper = new UserHelper(pdb.MockContext.Object);
            answerService = new AnswerService(db.MockContext.Object);
            lockService = new LockService(db.MockContext.Object, pdb.MockContext.Object, userHelper);

            bankAccountService = new BankAccountService(db.MockContext.Object, answerService, lockService);
        }

        [Test]
        public void SaveBankAccount()
        {
            // Arrange
            BankAccountCheck bankAccount = GetBankAccount();
            QuestionAnswerList questionAnswerList = GetQuestionAnswerList();

            // Act
            bankAccountService.SaveBankAccount(bankAccount, questionAnswerList);

            // Assert
            db.MockContext.Verify(x => x.SaveChanges(), Times.Exactly(4));
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
    }
}
