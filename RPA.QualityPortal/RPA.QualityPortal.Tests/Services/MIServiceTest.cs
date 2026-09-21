using Moq;
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
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class MIServiceTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;

        IOutboundCorrespondenceService outboundCorrespondenceService;
        IPeopleService peopleService;
        IAnswerService answerService;
        IMIService miService;
        IUserHelper userHelper;
        IFilterService filterService;
        Mock<IRoleManager> roleManager;
        ILockService lockService;

        [SetUp]
        public void Setup()
        { 
            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();

            roleManager = new Mock<IRoleManager>();
            answerService = new AnswerService(context.MockContext.Object);
            userHelper = new UserHelper(peopleContext.MockContext.Object);
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            miService = new MIService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, filterService);
            lockService = new LockService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper);
            outboundCorrespondenceService = new OutboundCorrespondenceService(context.MockContext.Object, answerService, userHelper, lockService);
            peopleService = new PeopleService(peopleContext.MockContext.Object);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());
        }

        [Test]

        public void Test_MISearch_returns_by_Both_Dates_Only()
        {
            //Arrange

            string DateFrom = DateTime.Now.AddDays(-1).ToString();
            string DateTo = DateTime.Now.AddDays(1).ToString();
            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("", "", DateFrom, DateTo, null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 4);
        }

        [Test]

        public void Test_MISearch_returns_by_DateFrom_Only()
        {
            //Arrange

            string DateFrom = DateTime.Now.AddDays(-1).ToString();
            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("", "", DateFrom, "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 4);
        }

        [Test]

        public void Test_MISearch_returns_by_DateTo_Only()
        {
            //Arrange

            string DateTo = DateTime.Now.AddDays(1).ToString();
            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("", "", "", DateTo, null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 4);
        }

        [Test]

        public void Test_MISearch_returns_by_QCName_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Toward, Fay", "QCName", "", "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 1);
        }

        [Test]

        public void Test_MISearch_returns_by_LineManager_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Toward, Fay", "LineManager", "", "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 2);
        }

        [Test]

        public void Test_MISearch_returns_by_QCChecker_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Toward, Fay", "QCChecker", "", "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 3);
        }

        [Test]

        public void Test_MISearch_returns_by_HEO_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Toward, Fay", "HEO", "", "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 3);
        }

        [Test]

        public void Test_MISearch_returns_by_SEO_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Toward, Fay", "SEO", "", "", null, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 3);
        }

        
        [Test]

        public void Test_MISearch_returns_by_Scheme_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch("Countryside Stewardship", "Scheme", null, null, 1, null, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 1);
        }

        [Test]

        public void Test_MISearch_returns_by_Business_Area_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch(null, "BusinessArea", null, null, null, 1, null, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 3);
        }

        [Test]

        public void Test_MISearch_returns_by_Correspondence_Type_Only()
        {
            //Arrange

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI();

            //Act

            var result = miService.OutboundCorrespondenceMISearch(null, "CorrespondenceType", null, null, null, null, 3, outboundCorrespondenceMI);

            //Assert

            Assert.AreEqual(result.OutboundCorrespondenceList.Count, 4);
        }

        [Test]

        public void Test_AnswerMIList_returns_List()
        {
            //Arrange

            //Act

            var result = miService.AnswerMIList();

            //Assert

            Assert.IsTrue(result.Count > 0);
        }


        [Test]

        public void Test_AnswerMISingleUserList_returns_List()
        {
            //Arrange

            string personName = "Gordon, [REDACTED_NAME]";

            //Act

            var result = miService.AnswerMIListSingleUser(personName);

            //Assert

            Assert.IsTrue(result.Count > 0);
        }




    }
}
