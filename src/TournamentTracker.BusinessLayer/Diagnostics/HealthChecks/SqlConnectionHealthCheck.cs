using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace TournamentTracker.BusinessLayer.Diagnostics.HealthChecks;

public class SqlConnectionHealthCheck : IHealthCheck
{
    private readonly IConfiguration configuration;
    private readonly ILogger<SqlConnectionHealthCheck> logger;

    public SqlConnectionHealthCheck(IConfiguration configuration, ILogger<SqlConnectionHealthCheck> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = configuration.GetConnectionString("SqlConnection");
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync(cancellationToken);
            await connection.CloseAsync();

            logger.LogInformation("healthy");
            return HealthCheckResult.Healthy();
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Unhealthy");
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}