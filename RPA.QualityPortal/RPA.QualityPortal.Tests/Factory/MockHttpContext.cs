using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace RPA.QualityPortal.Tests.Factory
{
    public static class MockHttpContext
    {
        public static HttpContextBase GetMockedHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            WindowsIdentity windowsIdentity = identity.Object as WindowsIdentity;


            var urlHelper = new Mock<UrlHelper>();

            var routes = new RouteCollection();
            RPA.QualityPortal.RouteConfig.RegisterRoutes(routes);
            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);

            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(x => x.Name).Returns("earth\\M600300");
            user.Setup(x => x.IsInRole("Quality Checks: OC Team Member")).Returns(true);
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            request.Setup(x => x.LogonUserIdentity).Returns(windowsIdentity);
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());
            request.SetupGet(req => req.Headers).Returns(new NameValueCollection());

            return context.Object;
        }
    }
}
