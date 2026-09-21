using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using NUnit.Framework;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Models;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using RPA.QualityPortal.ViewModels;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class OutboundCorrespondenceServiceTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;

        IOutboundCorrespondenceService outboundCorrespondenceService;
        IAnswerService answerService;
        IFilterService filterService;
        IRoleManager roleManager;
        IUserHelper userHelper;
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
        }

        [Test]
        public void Test_QCResult_Calculates_a_Result()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.IsNotNull(qaList.QCResult);
        }

        [Test]
        public void Test_QCResult_Correctly_Calculates_a_Fail()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.AreEqual(qaList.QCResult, "Not Approved");
        }
        [Test]
        public void Test_QCResult_Correctly_Calculates_a_Pass()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "Yes"
            };

            qaList.QuestionAnswers.Add(QA);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.AreEqual(qaList.QCResult, "Approved");
        }
        [Test]
        public void Test_QCResult_Correctly_Calculates_a_Pass_Advisory()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 1
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.AreEqual(qaList.QCResult, "Approved Advisory");
        }

        [Test]
        public void Test_QCResult_Correctly_Calculates_a_Pass_If_No_Questions_Answered_No()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 4
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "Yes"
            };

            qaList.QuestionAnswers.Add(QA);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.AreEqual(qaList.QCResult, "Approved");
        }

        [Test]
        public void Test_QCResult_Correctly_Calculates_a_Fail_When_Pass_Advisory_Also_Hit()
        {
            //Arrange

            QuestionAnswerList qaList = new QuestionAnswerList
            {
                QuestionAnswers = new List<QuestionAnswer>()
            };

            CheckQuestion CQ = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 1,
                Order = 4
            };


            CheckQuestion CQ1 = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 3,
                Order = 5
            };

            CheckQuestion CQ2 = new CheckQuestion
            {
                CheckTypeId = 1,
                QuestionId = 3,
                Order = 6
            };

            QuestionAnswer QA = new QuestionAnswer
            {
                CheckQuestion = CQ,
                Answer = "No"
            };

            QuestionAnswer QA1 = new QuestionAnswer
            {
                CheckQuestion = CQ1,
                Answer = "No"
            };

            qaList.QuestionAnswers.Add(QA);
            qaList.QuestionAnswers.Add(QA1);

            //Act

            outboundCorrespondenceService.QCResultCalculate(qaList);

            //Assert

            Assert.AreEqual(qaList.QCResult, "Not Approved");
        }


        [Test]
        public void Test_SaveOutboundCorrespondence_Saves_a_QC_Pass()
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

            //Arrange
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

            //Act

            outboundCorrespondenceService.SaveOutboundCorrespondence(outboundCorrespondence, qaList);

            //Assert

            context.MockContext.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Test_SaveOutboundCorrespondence_Saves_a_QC_Fail()
        {
            //Arrange
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

            //Arrange
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

            //Act

            outboundCorrespondenceService.SaveOutboundCorrespondence(outboundCorrespondence, qaList);

            //Assert

            context.MockContext.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [Test]
        public void Test_SaveOutboundCorrespondence_Saves_a_QC_Pass_Advisory()
        {
            //Arrange
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

            //Arrange
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

            //Act

            outboundCorrespondenceService.SaveOutboundCorrespondence(outboundCorrespondence, qaList);

            //Assert

            context.MockContext.Verify(x => x.SaveChanges(), Times.Exactly(2));
        }

        [Test]

        public void Test_SaveOutboundCorrespondence_Saves_Correct_ResultID()
        {
            //Arrange
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

            //Arrange
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

            //Act

            outboundCorrespondenceService.SaveOutboundCorrespondence(outboundCorrespondence, qaList);

            //Assert

            Assert.AreEqual(context.MockContext.Object.CheckResults.Where(x => x.CheckId == 2).Select(p => p.ResultId).FirstOrDefault(), 3);
        }

        [Test]
        public void Test_SaveEditOutboundCorrespondence_Saves_an_Edited_Correspondence()
        {
            //Arrange
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

            //Arrange
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

            //Act
            outboundCorrespondenceService.SaveEditOutboundCorrespondence(outboundCorrespondence, qaList, "Not Approved");

            //Assert

            context.MockContext.Verify(x => x.SaveChanges(), Times.Exactly(1));
        }

        [Test]
        public void Test_SaveEditOutboundCorrespondence_Saves_Correct_ResultId()
        {
            //Arrange
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

            //Arrange
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

            //Act

            outboundCorrespondenceService.SaveEditOutboundCorrespondence(outboundCorrespondence, qaList, "Not Approved");

            //Assert

            Assert.AreEqual(context.MockContext.Object.CheckResults.Where(x => x.CheckId == 2).Select(p => p.ResultId).FirstOrDefault(), 3);
        }

        [Test]
        public void Test_CheckDuplicates()
        {
            var result = outboundCorrespondenceService.CheckDuplicates("1234", 1, "123456", 1);

            Assert.AreEqual(result, true);
        }

        [Test]
        public void Test_SaveOutboundCorrespondence_Saves_a_Challenge()
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

            Challenge challenge = new Challenge
            {
                ChallengeDate = DateTime.Now,
                ChallengeOutcomeId = 2,
                CheckId = 1
            };

            qaList.QuestionAnswers.Add(QA);
            qaList.Challenge = true;

            //Arrange
            //Outbound Correspondence

            OutboundCorrespondence outboundCorrespondence = new OutboundCorrespondence
            {
                CheckId = 1,
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

            //Act

            outboundCorrespondenceService.SaveEditOutboundCorrespondence(outboundCorrespondence, qaList, "Approved");

            //Assert

            Assert.AreEqual(context.MockContext.Object.Challenge.Count(), 2);
        }

        [Test]
        public void Test_CheckEditsDuplicates()
        {
            var result = outboundCorrespondenceService.CheckEditDuplicates("1234", 1, "123456", 1, 0);

            Assert.AreEqual(result, true);
        }
    }
}
