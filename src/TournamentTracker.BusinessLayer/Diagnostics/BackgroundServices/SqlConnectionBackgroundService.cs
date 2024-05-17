using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TournamentTracker.BusinessLayer.Diagnostics.BackgroundServices;

public class SqlConnectionBackgroundService : BackgroundService
{
    private PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
    private readonly IConfiguration configuration;
    private readonly ILogger<SqlConnectionBackgroundService> logger;

    public SqlConnectionBackgroundService(IConfiguration configuration, ILogger<SqlConnectionBackgroundService> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connectionString = configuration.GetConnectionString("SqlConnection");

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var connection = new SqlConnection(connectionString);

            try
            {
                await connection.OpenAsync(stoppingToken);
                await connection.CloseAsync();

                logger.LogInformation("test connection succeeded");
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Unable to connect");
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }
    }
}