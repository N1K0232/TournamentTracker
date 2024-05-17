using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TournamentTracker.Core;

public class HealthCheckWriter
{
    public async Task WriteAsync(HttpContext context, HealthReport report)
    {
        var response = new
        {
            status = report.Status.ToString(),
            details = report.Entries.Select(e => new
            {
                service = e.Key,
                status = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                description = e.Value.Description
            })
        };

        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}