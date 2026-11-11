using Microsoft.AspNetCore.Localization;
using RPA.QualityPortal.Migrations;
using RPA.QualityPortal.Services;
using System.Data.Entity.Migrations;

var builder = WebApplication.CreateBuilder(args);

var qualityConnectionString = builder.Configuration.GetConnectionString("QualityContext")
    ?? throw new InvalidOperationException("Connection string 'QualityContext' is not configured.");
builder.Services.AddQualityPortalApplication(builder.Configuration);

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
