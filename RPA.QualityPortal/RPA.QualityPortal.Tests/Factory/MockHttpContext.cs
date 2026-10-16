using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace RPA.QualityPortal.Tests.Factory
{
    public static class MockHttpContext
    {
        public static IHttpContextAccessor GetMockedHttpContextAccessor()
        {
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "earth\\M600300"),
                    new Claim(ClaimTypes.Role, "Quality Checks: OC Team Member")
                }, "Test"))
            };

            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(x => x.HttpContext).Returns(context);

            return accessor.Object;
        }
    }
}
