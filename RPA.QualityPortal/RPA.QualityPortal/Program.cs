using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using RPA.QualityPortal.Migrations;
using RPA.QualityPortal.Services;
using System.Data.Entity.Migrations;

var builder = WebApplication.CreateBuilder(args);

var qualityConnectionString = builder.Configuration.GetConnectionString("QualityContext")
    ?? throw new InvalidOperationException("Connection string 'QualityContext' is not configured.");
builder.Services.AddQualityPortalApplication(builder.Configuration);

var app = builder.Build();
app.UseForwardedHeaders();

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

// Platform probes use HTTP even when the ingress terminates TLS. All application
// routes retain HTTPS redirection and secure authentication cookies.
app.UseWhen(context => context.Request.Path != "/health/live" && context.Request.Path != "/health/ready",
    application => application.UseHttpsRedirection());
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") })
    .AllowAnonymous();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
