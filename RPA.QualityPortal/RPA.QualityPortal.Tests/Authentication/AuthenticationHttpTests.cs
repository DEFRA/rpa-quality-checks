using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using NUnit.Framework;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System.Net;
using System.Security.Claims;

namespace RPA.QualityPortal.Tests.Authentication;

[TestFixture]
public class AuthenticationHttpTests
{
    private PortalFactory factory;
    private HttpClient client;

    [OneTimeSetUp]
    public void Setup()
    {
        factory = new PortalFactory();
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = false
        });
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        client?.Dispose();
        factory?.Dispose();
    }

    [Test]
    public async Task AnonymousRequestsChallengeWhileFailurePagesRemainAccessibleWithoutRedirectLoops()
    {
        var response = await client.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        StringAssert.StartsWith("https://login.microsoftonline.com/", response.Headers.Location.AbsoluteUri);
        StringAssert.Contains("response_type=code", response.Headers.Location.Query);
        Assert.AreEqual(HttpStatusCode.Unauthorized, (await client.GetAsync("/Account/AuthenticationFailed")).StatusCode);
        Assert.AreEqual(HttpStatusCode.Forbidden, (await client.GetAsync("/Account/AccessDenied")).StatusCode);
        using var tampered = new HttpRequestMessage(HttpMethod.Get, "/");
        tampered.Headers.Add("Cookie", "__Host-QualityPortal.Auth=tampered");
        Assert.AreEqual(HttpStatusCode.Redirect, (await client.SendAsync(tampered)).StatusCode);
    }

    [Test]
    public async Task RealCookiesAndViewsUseConfiguredPermissionsWithoutImplicitAdminInheritance()
    {
        var response = await Get("/", 1 | (1 << 2) | (1 << 6));
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        StringAssert.Contains("OutboundCorrespondence", html);
        StringAssert.Contains("BankAccount", html);
        AssertDenied(await Get("/BankAccount", 1 << 2));
        AssertDenied(await Get("/OutboundCorrespondence", 1 << 6));
        AssertDenied(await Get("/", 1 << 4));
        var empty = await Get("/", 1);
        Assert.AreEqual(HttpStatusCode.OK, empty.StatusCode);
        var emptyHtml = await empty.Content.ReadAsStringAsync();
        Assert.IsFalse(emptyHtml.Contains("href=\"/OutboundCorrespondence\""));
        Assert.IsFalse(emptyHtml.Contains("href=\"/BankAccount\""));
    }

    [Test]
    public async Task AdminRolesAreOrAndAccessPageHasNoIdtMutationForm()
    {
        foreach (var mask in new[] { 1 << 2, 1 << 6 })
        {
            var response = await Get("/Admin/Access", mask);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            StringAssert.Contains("managed externally", html);
            Assert.IsFalse(html.Contains("name=\"access\""));
            Assert.IsFalse(html.Contains("id=\"Psearch\""));
        }
        AssertDenied(await Get("/Admin/Access", 1 << 5));
        using var post = new HttpRequestMessage(HttpMethod.Post, "/Admin/Access");
        AddCookie(post, 1 << 6);
        post.Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "Example", ["access"] = "Admin" });
        Assert.AreEqual(HttpStatusCode.BadRequest, (await client.SendAsync(post)).StatusCode,
            "The retired action retains its antiforgery restriction.");
    }

    private static void AssertDenied(HttpResponseMessage response)
    {
        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        StringAssert.Contains("/Account/AccessDenied", response.Headers.Location.ToString());
    }

    private async Task<HttpResponseMessage> Get(string path, int mask)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        AddCookie(request, mask);
        return await client.SendAsync(request);
    }

    private void AddCookie(HttpRequestMessage request, int mask)
    {
        var options = factory.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>().Get("Cookies");
        var ticket = new AuthenticationTicket(PermissionTests.User(mask), "Cookies");
        request.Headers.Add("Cookie", options.Cookie.Name + "=" + options.TicketDataFormat.Protect(ticket));
    }

    private sealed class PortalFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(AuthenticationTests.Settings()));
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(AuthenticationTests.Settings()));
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IQualityContext>(_ => new MockQualityContext().MockContext.Object);
                services.AddScoped<IPeopleContext>(_ => new MockPeopleContext().MockContext.Object);
                services.PostConfigure<OpenIdConnectOptions>("OpenIdConnect", options =>
                {
                    options.Configuration = new OpenIdConnectConfiguration
                    {
                        AuthorizationEndpoint = "https://login.microsoftonline.com/test/oauth2/v2.0/authorize",
                        TokenEndpoint = "https://login.microsoftonline.com/test/oauth2/v2.0/token",
                        Issuer = "https://login.microsoftonline.com/" + AuthenticationTests.Tenant + "/v2.0"
                    };
                    options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(options.Configuration);
                });
            });
        }
    }
}
