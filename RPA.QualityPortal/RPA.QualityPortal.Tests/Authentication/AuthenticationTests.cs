using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Services;
using RPA.QualityPortal.Tests.DAL.Mock;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RPA.QualityPortal.Tests.Authentication;

[TestFixture]
public class AuthenticationTests
{
    internal const string Tenant = "e8af9db2-b595-4e8f-a691-ec5c8f131282";

    internal static Dictionary<string, string> Settings()
    {
        var settings = new Dictionary<string, string>
        {
            ["AzureAd:TenantId"] = Tenant,
            ["AzureAd:ClientId"] = "04c68ebf-c4f1-432e-b69c-b13b1f0ebc13",
            ["AzureAd:Instance"] = "https://login.microsoftonline.com/",
            ["AzureAd:ClientSecret"] = "test-only-not-a-credential",
            ["Identity:AuditNameClaimType"] = "staff",
            ["ConnectionStrings:QualityContext"] = "Data Source=.;Initial Catalog=UnusedQuality;Integrated Security=True",
            ["ConnectionStrings:PeopleContext"] = "Data Source=.;Initial Catalog=UnusedPeople;Integrated Security=True"
        };
        for (var index = 0; index < PermissionTests.Permissions.Length; index++)
            settings["Authorization:" + QualityPermissions.ConfigurationKeys[PermissionTests.Permissions[index]]] = "Test.Permission." + index;
        return settings;
    }

    internal static IConfiguration Configuration(Dictionary<string, string> settings = null) =>
        new ConfigurationBuilder().AddInMemoryCollection(settings ?? Settings()).Build();

    public static IEnumerable<string> RequiredKeys() => Settings().Keys;

    [TestCaseSource(nameof(RequiredKeys))]
    public void MissingConfigurationFailsClosed(string key)
    {
        var settings = Settings();
        settings[key] = " ";
        Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddQualityPortalApplication(Configuration(settings)));
    }

    [TestCase("AzureAd:TenantId", "common")]
    [TestCase("AzureAd:TenantId", "00000000-0000-0000-0000-000000000000")]
    [TestCase("AzureAd:ClientId", "not-an-id")]
    [TestCase("AzureAd:Instance", "http://login.microsoftonline.com/")]
    [TestCase("AzureAd:Instance", "https://user:password@login.microsoftonline.com/")]
    [TestCase("AzureAd:Instance", "https://login.microsoftonline.com/?x=1")]
    [TestCase("Identity:AuditNameClaimType", "staff number")]
    [TestCase("Authorization:BasicAccessRole", "Quality Checks: Basic Access")]
    [TestCase("Authorization:BasicAccessRole", ".invalid")]
    [TestCase("Authorization:BasicAccessRole", "Test.Permission.1")]
    public void InvalidConfigurationFailsClosed(string key, string value)
    {
        var settings = Settings();
        settings[key] = value;
        Assert.Throws<InvalidOperationException>(() => new ServiceCollection().AddQualityPortalApplication(Configuration(settings)));
    }

    [Test]
    public async Task OidcCookieAndClaimOptionsFollowCentralRegistration()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = Configuration();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddQualityPortalApplication(configuration);
        using var provider = services.BuildServiceProvider();
        var schemes = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        Assert.AreEqual("Cookies", (await schemes.GetDefaultAuthenticateSchemeAsync()).Name);
        Assert.AreEqual("OpenIdConnect", (await schemes.GetDefaultChallengeSchemeAsync()).Name);
        Assert.AreEqual("Cookies", (await schemes.GetDefaultForbidSchemeAsync()).Name);
        Assert.IsNull(await schemes.GetSchemeAsync("Negotiate"));
        var cookie = provider.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>().Get("Cookies");
        Assert.AreEqual("__Host-QualityPortal.Auth", cookie.Cookie.Name);
        Assert.IsTrue(cookie.Cookie.HttpOnly);
        Assert.AreEqual(CookieSecurePolicy.Always, cookie.Cookie.SecurePolicy);
        Assert.AreEqual(SameSiteMode.Lax, cookie.Cookie.SameSite);
        Assert.AreEqual("/", cookie.Cookie.Path);
        Assert.IsTrue(string.IsNullOrEmpty(cookie.Cookie.Domain));
        Assert.AreEqual("/Account/AccessDenied", cookie.AccessDeniedPath.Value);
        Assert.AreEqual(TimeSpan.FromMinutes(30), cookie.ExpireTimeSpan);
        Assert.IsTrue(cookie.SlidingExpiration);
        var oidc = provider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>().Get("OpenIdConnect");
        Assert.AreEqual("code", oidc.ResponseType);
        Assert.IsFalse(oidc.MapInboundClaims);
        Assert.AreEqual("roles", oidc.TokenValidationParameters.RoleClaimType);
        Assert.AreEqual("staff", oidc.TokenValidationParameters.NameClaimType);
        Assert.IsNotNull(oidc.Events.OnTokenValidated);
        Assert.IsNotNull(oidc.Events.OnRemoteFailure);
        Assert.IsFalse(((Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler)oidc.TokenHandler).MapInboundClaims);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task TokenValidatedEventEnforcesAuditContractBeforeIssuingACookie(bool valid)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = Configuration();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddQualityPortalApplication(configuration);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>().Get("OpenIdConnect");
        var principal = valid ? Principal("M600300") : Principal("someone@example.test");
        var http = new DefaultHttpContext { RequestServices = provider };
        var context = new TokenValidatedContext(http,
            new AuthenticationScheme("OpenIdConnect", null, typeof(OpenIdConnectHandler)), options, principal, new AuthenticationProperties())
        {
            Principal = principal,
            SecurityToken = new JwtSecurityToken(claims: principal.Claims),
            ProtocolMessage = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectMessage()
        };
        await options.Events.OnTokenValidated(context);
        Assert.AreEqual(!valid, context.Result?.Failure != null);
        Assert.IsFalse(http.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Test]
    public async Task RemoteFailureEventReturnsGenericAnonymousFailureEndpointWithoutLeakingDetails()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = Configuration();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddQualityPortalApplication(configuration);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIdConnectOptions>>().Get("OpenIdConnect");
        var http = new DefaultHttpContext { RequestServices = provider };
        var context = new RemoteFailureContext(http,
            new AuthenticationScheme("OpenIdConnect", null, typeof(OpenIdConnectHandler)), options,
            new Exception("sensitive provider failure details"));
        await options.Events.OnRemoteFailure(context);
        Assert.IsTrue(context.Result.Handled);
        Assert.AreEqual("/Account/AuthenticationFailed", http.Response.Headers.Location.ToString());
    }

    [TestCase("EARTH\\M0600300", "m600300")]
    [TestCase("demeter\\M0600300", "m600300")]
    [TestCase("M600300", "m600300")]
    [TestCase("m00123", "m0123")]
    public void AuditContractPreservesQualityPortalNormalization(string input, string expected)
    {
        var principal = Principal(input);
        Assert.AreEqual(expected, AuditIdentity.GetName(principal, "staff"));
        Assert.DoesNotThrow(() => AuditIdentity.Validate(principal, Guid.Parse(Tenant), "staff"));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("user@example.test")]
    [TestCase("John Smith")]
    [TestCase("other\\M600300")]
    public void AuditContractRejectsInvalidAccountValues(string value) =>
        Assert.Throws<InvalidOperationException>(() => AuditIdentity.Validate(Principal(value), Guid.Parse(Tenant), "staff"));

    [Test]
    public void TenantAndAuditContractsRejectMissingDuplicateOrAnonymousClaims()
    {
        var valid = Principal("M600300");
        var cases = new[]
        {
            new ClaimsPrincipal(new ClaimsIdentity()),
            new ClaimsPrincipal(new ClaimsIdentity(valid.Claims.Where(claim => claim.Type != "tid"), "test")),
            new ClaimsPrincipal(new ClaimsIdentity(valid.Claims.Append(new Claim("tid", Tenant)), "test")),
            new ClaimsPrincipal(new ClaimsIdentity(valid.Claims.Where(claim => claim.Type != "tid")
                .Append(new Claim("tid", Guid.NewGuid().ToString())), "test")),
            new ClaimsPrincipal(new ClaimsIdentity(valid.Claims.Where(claim => claim.Type != "staff"), "test")),
            new ClaimsPrincipal(new ClaimsIdentity(valid.Claims.Append(new Claim("staff", "M600300")), "test"))
        };
        foreach (var principal in cases)
            Assert.Throws<InvalidOperationException>(() => AuditIdentity.Validate(principal, Guid.Parse(Tenant), "staff"));
    }

    [Test]
    public void UserLookupRetainsPeopleDisplayNameAndUnknownFallbackInsteadOfUsingEntraDisplayName()
    {
        var people = new MockPeopleContext();
        var person = people.MockPeople.Object.Single(person => person.StaffNumber == "M600300");
        person.StaffNumber = "m600300";
        var context = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = Principal("EARTH\\M0600300") } };
        var helper = new UserHelper(people.MockContext.Object, context);
        Assert.AreEqual(person.Name, helper.CurrentUser());
        context.HttpContext.User = Principal("M999999");
        Assert.AreEqual("Unknown", helper.CurrentUser());
        context.HttpContext = null;
        Assert.AreEqual("Unknown", helper.CurrentUser());
    }

    internal static ClaimsPrincipal Principal(string value) =>
        new(new ClaimsIdentity([new Claim("staff", value), new Claim("tid", Tenant),
            new Claim("name", "Never use this display name for audit")], "test", "staff", "roles"));
}
