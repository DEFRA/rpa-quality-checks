using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RPA.QualityPortal.Services;

// Type-activated per connection string name (see ApplicationServices.AddHealthChecks), so the same
// class checks both QualityContext and PeopleContext without duplicating the ADO.NET probe logic.
internal sealed class SqlReadinessHealthCheck(string connectionStringName, IConfiguration configuration, ILogger<SqlReadinessHealthCheck> logger)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = new SqlConnectionStringBuilder(configuration.GetConnectionString(connectionStringName))
            {
                ConnectTimeout = 3
            };

            await using var connection = new SqlConnection(settings.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 3;

            return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) == 1
                ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception exception) when (exception is SqlException or InvalidOperationException or OperationCanceledException)
        {
            // SQL exceptions can contain server, database and identity information.
            logger.LogWarning(new EventId(1002, "SqlReadinessFailed"), "SQL readiness check failed for {ConnectionStringName}.", connectionStringName);

            return HealthCheckResult.Unhealthy();
        }
    }
}
