using Moq;
using NUnit.Framework;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class PeopleServiceTest
    {
        MockPeopleContext peopleContext;     

        IPeopleService peopleService;

        [SetUp]

        public void Setup()
        {
            peopleContext = new MockPeopleContext();
            peopleService = new PeopleService(peopleContext.MockContext.Object);
        }

        [Test]
        public void Test_PeopleService_Returns_True_With_Valid_Name()
        {
            var result = peopleService.PersonCheck("Gordon, Lee George");

            Assert.AreEqual(true, result);
        }

        [Test]
        public void Test_PeopleService_Returns_False_With_Invalid_Name()
        {
            var result = peopleService.PersonCheck("Beanface");

            Assert.AreEqual(false, result);
        }
    }
}