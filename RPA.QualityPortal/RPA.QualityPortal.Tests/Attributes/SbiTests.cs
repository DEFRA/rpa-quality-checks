using NUnit.Framework;
using RPA.QualityPortal.Attributes;

namespace RPA.QualityPortal.Tests.Attributes
{
    public class SbiTests
    {
        SBI sbiAttribute;

        [SetUp]
        public void Setup()
        {
            sbiAttribute = new SBI();
        }

        [Test]
        public void Test_SBI_Accepts_Valid_SBI()
        {
            var result = sbiAttribute.IsValid("105100100");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Unknown_Title_Case()
        {
            var result = sbiAttribute.IsValid("Unknown");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Unknown_Lower_Case()
        {
            var result = sbiAttribute.IsValid("unknown");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Unknown_Upper_Case()
        {
            var result = sbiAttribute.IsValid("UNKNOWN");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Multiple_Title_Case()
        {
            var result = sbiAttribute.IsValid("Multiple");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Multiple_Lower_Case()
        {
            var result = sbiAttribute.IsValid("multiple");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Accepts_Multiple_Upper_Case()
        {
            var result = sbiAttribute.IsValid("MULTIPLE");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_SBI_Rejects_Too_Low()
        {
            var result = sbiAttribute.IsValid("104100100");
            Assert.IsFalse(result);
        }

        [Test]
        public void Test_SBI_Rejects_Too_High()
        {
            var result = sbiAttribute.IsValid("1001001001");
            Assert.IsFalse(result);
        }

        [Test]
        public void Test_SBI_Rejects_Not_A_Number()
        {
            var result = sbiAttribute.IsValid("Invalid");
            Assert.IsFalse(result);
        }
    }
}
