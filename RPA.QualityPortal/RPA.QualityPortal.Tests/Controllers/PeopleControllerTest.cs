using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using RPA.QualityPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace RPA.QualityPortal.Tests.Controllers
{
    [TestFixture]
    public class PeopleControllerTest
    {
        MockPeopleContext context;
        PeopleController controller;
        IPeopleService peopleService;



        [SetUp]

        public void Setup()
        {


            context = new MockPeopleContext();
            peopleService = new PeopleService(context.MockContext.Object);


            controller = new PeopleController(context.MockContext.Object);
        }

        [Test]
        public void Test_PeopleSearch_Returns_Person_By_Name()
        {
            var result = controller.PeopleSearch("Gordon, [REDACTED_NAME]") as ContentResult;

            var actual = JsonConvert.DeserializeObject<IEnumerable<PeopleSearchModel>>(result.Content).ToList();

            Assert.AreEqual(actual[0].Label, "Gordon, [REDACTED_NAME] (LM: Slee, Alan)");
            Assert.AreEqual(actual[0].Value, "Gordon, [REDACTED_NAME]");
            Assert.AreEqual(actual[0].StaffNumber, "M600300");
            Assert.AreEqual(actual[0].Manager, "Slee, Alan");
        }

        [Test]
        public void Test_PeopleSearch_Returns_Person_By_Staff_Number()
        {
            var result = controller.PeopleSearch("Gordon, [REDACTED_NAME]") as ContentResult;

            var actual = JsonConvert.DeserializeObject<IEnumerable<PeopleSearchModel>>(result.Content).ToList();

            Assert.AreEqual(actual[0].Label, "Gordon, [REDACTED_NAME] (LM: Slee, Alan)");
            Assert.AreEqual(actual[0].Value, "Gordon, [REDACTED_NAME]");
            Assert.AreEqual(actual[0].StaffNumber, "M600300");
            Assert.AreEqual(actual[0].Manager, "Slee, Alan");
        }
    }
}
