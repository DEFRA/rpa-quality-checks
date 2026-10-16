using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Infrastructure;

namespace RPA.QualityPortal.Tests
{
    // Replaces MvcContrib.TestHelper.TestControllerBuilder: gives a hand-built controller a request context,
    // TempData and an in-memory session store.
    public class TestControllerBuilder
    {
        public void InitializeController(Controller controller)
        {
            var httpContext = new DefaultHttpContext();

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            if (controller is QualityController qualityController && qualityController.SessionStore == null)
            {
                // Keep whatever the test already stored in controller.Session
                qualityController.SessionStore = new InMemorySessionStore();
            }
        }
    }
}
