using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RPA.QualityPortal.Controllers;
using RPA.QualityPortal.Services;
using System.Reflection;
using System.Security.Claims;

namespace RPA.QualityPortal.Tests.Authentication;

[TestFixture]
public class PermissionTests
{
    private ServiceProvider provider;
    internal static readonly string[] Permissions =
    [
        QualityPermissions.BasicAccess, QualityPermissions.BankMember, QualityPermissions.BankAdmin,
        QualityPermissions.BankLeader, QualityPermissions.OutboundMember, QualityPermissions.OutboundManager,
        QualityPermissions.OutboundAdmin
    ];

    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQualityPortalApplication(AuthenticationTests.Configuration());
        provider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void Cleanup() => provider.Dispose();

    [TestCaseSource(nameof(Masks))]
    public async Task EveryRoleCombinationPreservesPoliciesNavigationAndServiceChecks(int mask)
    {
        var user = User(mask);
        var authorization = provider.GetRequiredService<IAuthorizationService>();
        var roleManager = new RoleManager(new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = user } },
            provider.GetRequiredService<QualityPermissions>());

        for (var index = 0; index < Permissions.Length; index++)
        {
            var expected = (mask & (1 << index)) != 0;
            Assert.AreEqual(expected, roleManager.IsUserInRole(Permissions[index]), Permissions[index]);
            Assert.AreEqual(expected, (await authorization.AuthorizeAsync(user, null, Permissions[index])).Succeeded);
        }
        Assert.AreEqual((mask & ((1 << 4) | (1 << 5))) != 0,
            (await authorization.AuthorizeAsync(user, null, QualityPermissions.OutboundAccess)).Succeeded);
        Assert.AreEqual((mask & ((1 << 2) | (1 << 6))) != 0,
            (await authorization.AuthorizeAsync(user, null, QualityPermissions.AdminAccess)).Succeeded);
        var combined = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
            .RequireAssertion(context => provider.GetRequiredService<QualityPermissions>()
                .HasPermission(context.User, QualityPermissions.BasicAccess))
            .RequireAssertion(context => provider.GetRequiredService<QualityPermissions>()
                .HasPermission(context.User, QualityPermissions.BankMember)).Build();
        Assert.AreEqual((mask & 3) == 3, (await authorization.AuthorizeAsync(user, null, combined)).Succeeded,
            "Multiple permission requirements remain AND, not OR.");
    }

    public static IEnumerable<int> Masks() => Enumerable.Range(0, 1 << Permissions.Length);

    internal static ClaimsPrincipal User(int mask, string authenticationType = "test") =>
        new(new ClaimsIdentity(new[] { new Claim("staff", "earth\\M0600300"),
            new Claim("tid", AuthenticationTests.Tenant) }
            .Concat(Permissions.Select((_, index) => index)
                .Where(index => (mask & (1 << index)) != 0)
                .Select(index => new Claim("roles", "Test.Permission." + index))),
            authenticationType, "staff", "roles"));

    [Test]
    public async Task AnonymousUnknownLegacyAndCaseMismatchedRolesNeverGrantAccess()
    {
        var authorization = provider.GetRequiredService<IAuthorizationService>();
        foreach (var claim in new[]
        {
            new Claim("roles", QualityPermissions.BasicAccess),
            new Claim(ClaimTypes.Role, "Test.Permission.0"),
            new Claim("roles", "test.permission.0"),
            new Claim("roles", "Unmapped.Role")
        })
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity([claim], "test"));
            Assert.IsFalse((await authorization.AuthorizeAsync(user, null, QualityPermissions.BasicAccess)).Succeeded);
        }
        Assert.IsFalse((await authorization.AuthorizeAsync(User(127, null), null, QualityPermissions.AdminAccess)).Succeeded);
        Assert.IsFalse(provider.GetRequiredService<QualityPermissions>().HasPermission(User(127), "unmapped"));
        Assert.IsFalse(new RoleManager(new HttpContextAccessor(), provider.GetRequiredService<QualityPermissions>())
            .IsUserInRole(QualityPermissions.BasicAccess));
    }

    [TestCase("HomeController", QualityPermissions.BasicAccess)]
    [TestCase("BankAccountController", QualityPermissions.BankMember)]
    [TestCase("BankAccountReCheckController", QualityPermissions.BankMember)]
    [TestCase("OutboundCorrespondenceController", QualityPermissions.OutboundAccess)]
    [TestCase("OutboundCorrespondenceReCheckController", QualityPermissions.OutboundAccess)]
    [TestCase("OutboundCorrespondenceMIController", QualityPermissions.OutboundAccess)]
    [TestCase("AdminController", QualityPermissions.AdminAccess)]
    public void ControllerPolicyContractsAreUnchanged(string name, string policy)
    {
        var controller = typeof(HomeController).Assembly.GetTypes().Single(type => type.Name == name);
        var attributes = controller.GetCustomAttributes<AuthorizeAttribute>(true).ToArray();
        Assert.AreEqual(1, attributes.Length);
        Assert.AreEqual(policy, attributes[0].Policy);
        Assert.IsNull(attributes[0].Roles);
    }

    [Test]
    public async Task UnannotatedControllersStillRequireAuthenticationAndErrorEndpointsAreAnonymous()
    {
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthorizationOptions>>().Value;
        var authorization = provider.GetRequiredService<IAuthorizationService>();
        Assert.IsFalse((await authorization.AuthorizeAsync(new ClaimsPrincipal(new ClaimsIdentity()), null, options.FallbackPolicy)).Succeeded);
        Assert.IsTrue((await authorization.AuthorizeAsync(User(0), null, options.FallbackPolicy)).Succeeded);
        Assert.IsTrue(typeof(AccountController).IsDefined(typeof(AllowAnonymousAttribute), true));
    }
}
