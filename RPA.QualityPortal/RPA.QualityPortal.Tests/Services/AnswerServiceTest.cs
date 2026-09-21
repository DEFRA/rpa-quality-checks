using NUnit.Framework;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class AnswerServiceTest
    {
        MockQualityContext db;
        AnswerService answerService;

        [SetUp]
        public void Setup()
        {
            db = new MockQualityContext();
            answerService = new AnswerService(db.MockContext.Object);
        }


        [Test]
        public void CommentCheck()
        {
            //Arrange
            string comment = "Test Change \" to '";

            //Act
            var result = answerService.CheckComment(comment);

            //Assert
            Assert.AreEqual(result, "Test Change ' to '");
        }
    }
}