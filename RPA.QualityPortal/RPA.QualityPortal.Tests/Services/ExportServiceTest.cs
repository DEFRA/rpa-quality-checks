using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System;
using System.IO;
using System.Reflection;

namespace RPA.QualityPortal.Tests.Services
{
    [TestFixture]
    public class ExportServiceTest
    {
        MockQualityContext context;
        MockPeopleContext peopleContext;
        IFilterService filterService;
        IUserHelper userHelper;
        Mock<IRoleManager> roleManager;
        IExportService exportService;

        [SetUp]
        public void Setup()
        {
            context = new MockQualityContext();
            peopleContext = new MockPeopleContext();
            roleManager = new Mock<IRoleManager>();
            filterService = new FilterService(context.MockContext.Object, peopleContext.MockContext.Object, userHelper, roleManager.Object);
            exportService = new ExportService(context.MockContext.Object);
        }
    }
}
