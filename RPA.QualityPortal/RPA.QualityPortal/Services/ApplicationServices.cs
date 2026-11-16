using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Infrastructure;

namespace RPA.QualityPortal.Services;

public static class ApplicationServices
{
    public static IServiceCollection AddQualityPortalApplication(this IServiceCollection services, IConfiguration configuration)
    {
        if (!Guid.TryParse(Required(configuration, "AzureAd:TenantId"), out var tenantId) || tenantId == Guid.Empty)
            throw new InvalidOperationException("AzureAd:TenantId must identify a single tenant.");
        if (!Guid.TryParse(Required(configuration, "AzureAd:ClientId"), out var clientId) || clientId == Guid.Empty)
            throw new InvalidOperationException("AzureAd:ClientId must be an application registration ID.");
        if (!Uri.TryCreate(Required(configuration, "AzureAd:Instance"), UriKind.Absolute, out var instance) ||
            instance.Scheme != Uri.UriSchemeHttps || !string.IsNullOrEmpty(instance.UserInfo) ||
            !string.IsNullOrEmpty(instance.Query) || !string.IsNullOrEmpty(instance.Fragment))
            throw new InvalidOperationException("AzureAd:Instance must be the approved HTTPS identity authority.");
        Required(configuration, "AzureAd:ClientSecret");
        var auditClaim = Required(configuration, "Identity:AuditNameClaimType");
        if (auditClaim.Any(char.IsWhiteSpace))
            throw new InvalidOperationException("Identity:AuditNameClaimType must be an explicit claim type without whitespace.");

        var roles = QualityPermissions.ConfigurationKeys.ToDictionary(
            pair => pair.Key, pair => RequiredRole(configuration, pair.Value), StringComparer.Ordinal);
        if (roles.Values.Distinct(StringComparer.Ordinal).Count() != roles.Count)
            throw new InvalidOperationException("Quality Portal permissions must map to seven distinct Entra app-role values.");
        var permissions = new QualityPermissions(roles);
        var qualityConnection = Required(configuration, "ConnectionStrings:QualityContext");
        var peopleConnection = Required(configuration, "ConnectionStrings:PeopleContext");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));
        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
            options.MapInboundClaims = false;
            if (options.TokenHandler is Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler tokenHandler)
                tokenHandler.MapInboundClaims = false;
            options.TokenValidationParameters.RoleClaimType = "roles";
            options.TokenValidationParameters.NameClaimType = auditClaim;
            var previous = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                if (previous != null) await previous(context);
                if (context.Result?.Failure != null) return;
                try
                {
                    AuditIdentity.Validate(context.Principal, tenantId, auditClaim);
                }
                catch (InvalidOperationException)
                {
                    LogAuthenticationFailure(context.HttpContext, "IDENTITY_CONTRACT_REJECTED");
                    context.Fail("The identity is missing its required tenant or staff-number audit contract.");
                }
            };
            var previousFailure = options.Events.OnRemoteFailure;
            options.Events.OnRemoteFailure = async context =>
            {
                LogAuthenticationFailure(context.HttpContext, "OIDC_REMOTE_FAILURE");
                if (previousFailure != null) await previousFailure(context);
                if (context.Result?.Handled == true || context.Result?.Skipped == true) return;
                context.HandleResponse();
                context.Response.Redirect("/Account/AuthenticationFailed");
            };
        });
        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "__Host-QualityPortal.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Path = "/";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        });
        services.AddSingleton(permissions);
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            foreach (var permission in roles.Keys)
                options.AddPolicy(permission, policy => policy.RequireAuthenticatedUser()
                    .RequireAssertion(context => permissions.HasPermission(context.User, permission)));
            options.AddPolicy(QualityPermissions.OutboundAccess, policy => policy.RequireAuthenticatedUser()
                .RequireAssertion(context => permissions.HasPermission(context.User, QualityPermissions.OutboundMember) ||
                                             permissions.HasPermission(context.User, QualityPermissions.OutboundManager)));
            options.AddPolicy(QualityPermissions.AdminAccess, policy => policy.RequireAuthenticatedUser()
                .RequireAssertion(context => permissions.HasPermission(context.User, QualityPermissions.OutboundAdmin) ||
                                             permissions.HasPermission(context.User, QualityPermissions.BankAdmin)));
        });
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddQualityPortalDataProtection(configuration);
        services.AddQualityPortalApplicationInsights(configuration);
        services.AddQualityPortalForwarding(configuration);
        services.AddHttpsRedirection(options => options.HttpsPort = RequiredHttpsPort(configuration));
        services.AddHealthChecks()
            .AddTypeActivatedCheck<SqlReadinessHealthCheck>("quality-sql", failureStatus: null, tags: ["ready"], args: ["QualityContext"])
            .AddTypeActivatedCheck<SqlReadinessHealthCheck>("people-sql", failureStatus: null, tags: ["ready"], args: ["PeopleContext"]);
        services.AddScoped<ISessionStore, SessionStore>();
        services.AddScoped<IQualityContext>(_ => new QualityContext(qualityConnection));
        services.AddScoped<IPeopleContext>(_ => new PeopleContext(peopleConnection));
        services.AddScoped<IUserHelper, UserHelper>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<ISendService, SendService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IBankAccountService, BankAccountService>();
        services.AddScoped<IMessageService, EmailService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IFilterService, FilterService>();
        services.AddScoped<ILockService, LockService>();
        services.AddScoped<IMIService, MIService>();
        services.AddScoped<IOutboundCorrespondenceService, OutboundCorrespondenceService>();
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddControllersWithViews();
        return services;
    }

    private static int RequiredHttpsPort(IConfiguration configuration)
    {
        if (!int.TryParse(configuration["HttpsRedirection:HttpsPort"] ?? "443", out var httpsPort) || httpsPort is < 1 or > 65535)
            throw new InvalidOperationException("HttpsRedirection:HttpsPort must be a valid HTTPS port.");
        return httpsPort;
    }

    private static string Required(IConfiguration configuration, string key) =>
        !string.IsNullOrWhiteSpace(configuration[key]) ? configuration[key] :
            throw new InvalidOperationException($"Required external configuration '{key}' is missing.");

    private static string RequiredRole(IConfiguration configuration, string name)
    {
        var role = Required(configuration, "Authorization:" + name);
        if (role.Length > 120 || role[0] == '.' ||
            role.Any(c => !char.IsAsciiLetterOrDigit(c) && !".-_!#$%&'()*+,/:;<=>?@[]^`{|}~".Contains(c)))
            throw new InvalidOperationException($"Authorization:{name} must be an Entra app-role token value, not its display name.");
        return role;
    }

    private static void LogAuthenticationFailure(HttpContext context, string code) =>
        context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("RPA.QualityPortal.Authentication")
            .LogWarning(new EventId(1001, "AuthenticationRejected"), "Authentication rejected. FailureCode: {FailureCode}", code);
}
