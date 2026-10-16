using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IIS;
using RPA.QualityPortal;
using RPA.QualityPortal.DAL;
using RPA.QualityPortal.Helpers;
using RPA.QualityPortal.Infrastructure;
using RPA.QualityPortal.Migrations;
using RPA.QualityPortal.Services;
using System.Data.Entity.Migrations;

var builder = WebApplication.CreateBuilder(args);

var qualityConnectionString = builder.Configuration.GetConnectionString("QualityContext")
    ?? throw new InvalidOperationException("Connection string 'QualityContext' is not configured.");
var peopleConnectionString = builder.Configuration.GetConnectionString("PeopleContext")
    ?? throw new InvalidOperationException("Connection string 'PeopleContext' is not configured.");

// IIS in-process hosting authenticates the user itself; Kestrel needs the Negotiate handler.
var hostedInIis = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APP_POOL_ID"));
if (hostedInIis)
{
    builder.Services.AddAuthentication(IISServerDefaults.AuthenticationScheme);
}
else
{
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
}

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IClaimsTransformation, IdtRoleClaimsTransformation>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionStore, SessionStore>();

builder.Services.AddScoped<IQualityContext>(_ => new QualityContext(qualityConnectionString));
builder.Services.AddScoped<IPeopleContext>(_ => new PeopleContext(peopleConnectionString));

builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<ISendService, SendService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IMessageService, EmailService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IFilterService, FilterService>();
builder.Services.AddScoped<ILockService, LockService>();
builder.Services.AddScoped<IMIService, MIService>();
builder.Services.AddScoped<IOutboundCorrespondenceService, OutboundCorrespondenceService>();
builder.Services.AddScoped<IPeopleService, PeopleService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Schema changes are normally applied by the deployment pipeline; this switch keeps the previous
// "migrate on start-up" behaviour available for environments that still need it.
if (builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    new DbMigrator(new Configuration(qualityConnectionString)).Update();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en-GB")
    .AddSupportedCultures("en-GB")
    .AddSupportedUICultures("en-GB"));

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
