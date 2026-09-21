using Moq;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using RPA.QualityPortal.Tests.Factory;
using RPA.QualityPortal.Factory;
using RPA.QualityPortal.ViewModels.OutboundCorrespondenceVM.MI;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using RPA.QualityPortal.Tests.Data.Mock;
using RPA.QualityPortal.Models.CheckTypes;
using RPA.QualityPortal.Models.OutboundCorrespondenceDropDowns;
using RPA.QualityPortal.Models;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class OutboundCorrespondenceMITest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        OutboundCorrespondenceMIController controller;
        IMIService miService;
        IUserHelper userHelper;
        IFilterService filterService;
        Mock<IRoleManager> roleManager;

        [SetUp]
        public void Setup()
        {
            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();
            roleManager = new Mock<IRoleManager>();
            userHelper = new UserHelper(peopleContext.MockContext.Object);
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            miService = new MIService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, filterService);

            HttpContextManager.SetCurrentContext(MockHttpContext.GetMockedHttpContext());

            controller = new OutboundCorrespondenceMIController(context.MockContext.Object, peopleContext.MockContext.Object, miService, userHelper, filterService, roleManager.Object);
        }

        [Test]

        public void Test_OutboundCorrespondenceMI_Returns_View_with_No_Input()
        {

            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            //Act

            var result = controller._OutboundCorrespondenceMI("", "", "", "") as PartialViewResult;

            //Assert

            Assert.AreEqual(4, ((OutboundCorrespondenceMI)result.ViewData.Model).OutboundCorrespondenceList.Count());

        }

        [Test]

        public void Test_OutboundCorrespondenceMI_Returns_View_with_SearchString_QC()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();

            List<OutboundCorrespondence> outboundCorrespondenceList = new List<OutboundCorrespondence>
            {
                new OutboundCorrespondence
                {
                    CheckId = 2,
                    PersonName = "Dormand, Scott",
                    QCCompletedByName = "Dormand, Scott",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 1,
                    CheckType = new CheckType(),
                    BusinessAreaId = 1,
                    BusinessArea = new BusinessArea(),
                    CorrespondenceTypeId = 3,
                    CorrespondenceType = new CorrespondenceType{ CorrespondenceTypeId = 3, Text =  "Letter - Template", Active = true },
                    CRMRefPrefixId = 1,
                    CRMRefPrefix = new CRMRefPrefix { CRMRefPrefixId = 1, Text = "Test", Active = true },
                    FailReasonId = 1,
                    FailReason = new FailReason(),
                    SchemeId = 1,
                    Scheme = new Scheme { SchemeId = 1, Text = "Countryside Stewardship", Active = true },
                    UniqueIdentifierPrefixId = 1,
                    UniqueIdentifierPrefix = new UniqueIdentifierPrefix{ UniqueIdentifierPrefixId = 1, Text = "CNF -", Active = true },
                    TemplateReference = "7654321",
                    UniqueId = "4321",
                    SBI = "987654321",
                    CRMRef = "654321",
                    OutboundCorrespondenceDateSent = DateTime.Now,
                    ManagerName = "Dormand, Scott",
                    HEO = "Dormand, Scott",
                    SEO = "Dormand, Scott",
                    Comments = "Test 2",
                    ReCheckRequired = false,
                    ExcludeQCResult = false
                }
            };

            OutboundCorrespondenceMIStats outboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats
            {
                TotalQcs = 0,
                Days = 0,
                Passed = 0,
                PassedPercentage = 0,
                PassAdvisory = 0,
                PassedAdvisoryPercentage = 0,
                Failed = 0,
                FailedPercentage = 0,
                OutstandingZeroToFive = 0,
                OutstandingSixToTen = 0,
                OutstandingGreaterThanTen = 0,
                QcsOutstanding = 0,
                ReChecks = 0,
                ReCheckPassed = 0,
                ReCheckPassedPercentage = 0,
                ReCheckPassAdvisory = 0,
                ReCheckPassedAdvisoryPercentage = 0,
                ReCheckFailed = 0,
                ReCheckFailedPercentage = 0,
                ReCheckOutstandingZeroToFive = 0,
                ReCheckOutstandingSixToTen = 0,
                ReCheckOutstandingGreaterThanTen = 0,
                ReChecksOutstanding = 0,
                ReChecksRequired = 0,
                ReChecksNotRequired = 0
            };

            List<OutboundCorrespondenceMIPersonStats> outboundCorrespondenceMIPersonStatsList = new List<OutboundCorrespondenceMIPersonStats>
            {
                new OutboundCorrespondenceMIPersonStats
                {
                    PersonName = "Dormand, Scott",
                    LineManagerName = "Dormand, Scott",
                    HEOName = "Dormand, Scott",
                    SEOName = "Dormand, Scott",
                    OutboundCorrespondenceMIStats = outboundCorrespondenceMIStats
                }
            };

            List<OutboundCorrespondenceReCheck> outboundCorrespondenceReCheckList = new List<OutboundCorrespondenceReCheck>
            {
                new  OutboundCorrespondenceReCheck
                {
                    CheckId = 2,
                    PersonName = "Dormand, Scott",
                    QCCompletedByName = "Dormand, Scott",
                    DateQCCompleted = DateTime.Now,
                    Archived = false,
                    CheckTypeId = 2,
                    EmailNotifications = true
                }
            };

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                OutboundCorrespondenceList = outboundCorrespondenceList,
                OutboundCorrespondenceMIPersonStatsList = outboundCorrespondenceMIPersonStatsList,
                OutboundCorrespondenceMIStats = outboundCorrespondenceMIStats,
                OutboundCorrespondenceReCheckList = outboundCorrespondenceReCheckList
            };

            builder.InitializeController(controller);

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller._OutboundCorrespondenceMI("Dormand, Scott", "LineManager", "", "") as PartialViewResult;

            //Assert

            Assert.AreEqual("Dormand, Scott", ((OutboundCorrespondenceMI)result.ViewData.Model).OutboundCorrespondenceList[0].PersonName);
        }

        [Test]
        public void Test_OutboundCorrespondenceMI_Returns_SessionData_when_navigated_Via_Back()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test"
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller._OutboundCorrespondenceMI("Toward, Fay", "LineManager", "", "", back: true) as PartialViewResult;

            //Assert

            Assert.AreEqual("Session Test", ((OutboundCorrespondenceMI)result.ViewData.Model).ResultHeader);
        }

        [Test]
        public void Test_OutboundCorrespondenceMI_Returns_SessionData_when_navigated_Via_Back_with_no_Search()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test"
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller._OutboundCorrespondenceMI("", "", "", "", back: true) as PartialViewResult;

            //Assert

            Assert.AreEqual("Session Test", ((OutboundCorrespondenceMI)result.ViewData.Model).ResultHeader);
        }

        [Test]

        public void Test_MI_Details_Returns_MI_Details_no_Person_Name()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test",
                DateTo = DateTime.Now,
                DateFrom = DateTime.Now.AddDays(-30),
                OutboundCorrespondenceList = OutboundCorrespondenceData.Data(),
                OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 }
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller.MIDetails("") as ViewResult;

            //Assert

            Assert.AreEqual(1, ((OutboundCorrespondenceMIDetails)result.ViewData.Model).FailReasonCount.Count);
        }
        [Test]

        public void Test_MI_Details_Returns_MI_Details_with_Person_Name_SEO()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test",
                DateTo = DateTime.Now,
                DateFrom = DateTime.Now.AddDays(-30),
                OutboundCorrespondenceList = OutboundCorrespondenceData.Data(),
                OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 },
                SearchType = "SEO",
                OutboundCorrespondenceMIPersonStatsList = new List<OutboundCorrespondenceMIPersonStats>
                {
                    new OutboundCorrespondenceMIPersonStats { HEOName = "Toward, Fay", OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 } }
                }
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller.MIDetails("Toward, Fay") as ViewResult;

            //Assert

            Assert.AreEqual(1, ((OutboundCorrespondenceMIDetails)result.ViewData.Model).FailReasonCount.Count);
        }

        [Test]
        public void Test_MI_Details_Returns_MI_Details_with_Person_Name_HEO()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test",
                DateTo = DateTime.Now,
                DateFrom = DateTime.Now.AddDays(-30),
                OutboundCorrespondenceList = OutboundCorrespondenceData.Data(),
                OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 },
                SearchType = "HEO",
                OutboundCorrespondenceMIPersonStatsList = new List<OutboundCorrespondenceMIPersonStats>
                {
                    new OutboundCorrespondenceMIPersonStats { LineManagerName = "Toward, Fay", OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 } }
                }
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller.MIDetails("Toward, Fay") as ViewResult;

            //Assert

            Assert.AreEqual(1, ((OutboundCorrespondenceMIDetails)result.ViewData.Model).FailReasonCount.Count);
        }

        [Test]
        public void Test_MI_Details_Returns_MI_Details_with_Person_Name()
        {
            //Arrange

            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);

            OutboundCorrespondenceMI outboundCorrespondenceMI = new OutboundCorrespondenceMI
            {
                ResultHeader = "Session Test",
                DateTo = DateTime.Now,
                DateFrom = DateTime.Now.AddDays(-30),
                OutboundCorrespondenceList = OutboundCorrespondenceData.Data(),
                OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 },
                SearchType = "Line Manager",
                OutboundCorrespondenceMIPersonStatsList = new List<OutboundCorrespondenceMIPersonStats>
                {
                    new OutboundCorrespondenceMIPersonStats { PersonName = "Toward, Fay", OutboundCorrespondenceMIStats = new OutboundCorrespondenceMIStats { Failed = 1, FailedPercentage = 50 } }
                }
            };

            controller.Session["MIData"] = outboundCorrespondenceMI;

            //Act

            var result = controller.MIDetails("Toward, Fay") as ViewResult;

            //Assert

            Assert.AreEqual(1, ((OutboundCorrespondenceMIDetails)result.ViewData.Model).FailReasonCount.Count);
        }

        [Test]

        public void Test_MIIndex_Returns_View()
        {
            TestControllerBuilder builder = new TestControllerBuilder();
            builder.InitializeController(controller);
            controller.Session["MIPage"] = "";

            //Act

            var result = controller.MIIndex() as ViewResult;

            //Assert

            Assert.IsNotNull(result);

        }

        [Test]

        public void Test_AnswerMI_Returns_View()
        {
            //Act

            var result = controller._AnswerMI() as PartialViewResult;

            //Assert

            Assert.IsNotNull(result);

        }
    }


}
