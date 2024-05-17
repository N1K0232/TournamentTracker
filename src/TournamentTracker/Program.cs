using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalHelpers.OpenApi;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.Swagger;
using TinyHelpers.Extensions;
using TinyHelpers.Json.Serialization;
using TournamentTracker.BusinessLayer.Diagnostics.BackgroundServices;
using TournamentTracker.BusinessLayer.Diagnostics.HealthChecks;
using TournamentTracker.BusinessLayer.Mapping;
using TournamentTracker.BusinessLayer.Services;
using TournamentTracker.BusinessLayer.Settings;
using TournamentTracker.BusinessLayer.Validations;
using TournamentTracker.Core;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.ExceptionHandlers;
using TournamentTracker.Extensions;
using TournamentTracker.Http;
using TournamentTracker.StorageProviders.Extensions;
using TournamentTracker.Swagger;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

ConfigureServices(builder.Services, builder.Configuration, builder.Environment, builder.Host);

var app = builder.Build();
Configure(app, app.Environment, app.Services);

await app.RunAsync();

void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, IHostBuilder host)
{
    var appSettings = services.ConfigureAndGet<AppSettings>(configuration, nameof(AppSettings));
    var swaggerSettings = services.ConfigureAndGet<SwaggerSettings>(configuration, nameof(SwaggerSettings));

    services.AddRazorPages();
    services.AddWebOptimizer(minifyCss: true, minifyJavaScript: environment.IsProduction());

    services.AddMemoryCache();
    services.AddHttpContextAccessor();

    services.AddRequestLocalization(appSettings.SupportedCultures);
    services.AddExceptionHandler<DefaultExceptionHandler>();
    services.AddSingleton<HealthCheckWriter>();

    services.AddResiliencePipeline("timeout", (builder, context) =>
    {
        builder.AddTimeout(new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(2),
            OnTimeout = args =>
            {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Timeout occurred after: {TotalSeconds} seconds", args.Timeout.TotalSeconds);

                return default;
            }
        });
    });

    services.AddResiliencePipeline<string, HttpResponseMessage>("http", (builder, context) =>
    {
        builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 3,
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(r => r.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests or >= HttpStatusCode.InternalServerError),
            DelayGenerator = args =>
            {
                if (args.Outcome.Result is not null && args.Outcome.Result.Headers.TryGetValues(HeaderNames.RetryAfter, out var value))
                {
                    return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(int.Parse(value.First())));
                }

                return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(Math.Pow(2, args.AttemptNumber + 1)));
            },
            OnRetry = args =>
            {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Retrying... {AttemptNumber} attempt after {RetryDelay}", args.AttemptNumber + 1, args.RetryDelay);

                return default;
            }
        });
    });

    services.AddTransient<TransientErrorDelegatingHandler>();
    services.AddHttpClient("http").AddHttpMessageHandler<TransientErrorDelegatingHandler>();

    services.AddOperationResult(options =>
    {
        options.ErrorResponseFormat = ErrorResponseFormat.List;
    });

    services.AddAutoMapper(typeof(TournamentMapperProfile).Assembly);
    services.AddValidatorsFromAssemblyContaining<SaveTournamentRequestValidator>();

    services.AddFluentValidationAutoValidation(options =>
    {
        options.DisableDataAnnotationsValidation = true;
    });

    services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            var statusCode = context.ProblemDetails.Status.GetValueOrDefault(StatusCodes.Status500InternalServerError);
            context.ProblemDetails.Type ??= $"https://httpstatuses.io/{statusCode}";
            context.ProblemDetails.Title ??= ReasonPhrases.GetReasonPhrase(statusCode);
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
            context.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        };
    });

    if (swaggerSettings.Enabled)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(swaggerSettings.Version, new OpenApiInfo { Title = swaggerSettings.Title, Version = swaggerSettings.Version });
            options.AddDefaultResponse();
            options.AddAcceptLanguageHeader();
            options.AddFormFile();
        })
        .AddFluentValidationRulesToSwagger(options =>
        {
            options.SetNotNullableIfMinLengthGreaterThenZero = true;
        });
    }

    var connectionString = configuration.GetConnectionString("SqlConnection");
    services.AddScoped<IDataContext>(services => services.GetRequiredService<DataContext>());
    services.AddSqlServer<DataContext>(connectionString, options =>
    {
        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        options.CommandTimeout(appSettings.CommandTimeout);
        options.EnableRetryOnFailure(appSettings.MaxRetryCount, appSettings.MaxRetryDelay, null);
    });

    var azureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnection");
    if (azureStorageConnectionString.HasValue() && appSettings.ContainerName.HasValue())
    {
        services.AddAzureStorage(options =>
        {
            options.UseConnectionString(azureStorageConnectionString);
            options.UseContainerName(appSettings.ContainerName);
        });
    }
    else
    {
        services.AddFileSystemStorage(options =>
        {
            options.UseStorageFolder(appSettings.StorageFolder ?? AppContext.BaseDirectory);
        });
    }

    services.AddHealthChecks().AddCheck<SqlConnectionHealthCheck>("sql");
    services.AddHostedService<SqlConnectionBackgroundService>();

    services.Scan(scan => scan.FromAssemblyOf<TournamentService>()
        .AddClasses(classes => classes.InNamespaceOf<TournamentService>())
        .AsImplementedInterfaces()
        .WithScopedLifetime());
}

void Configure(IApplicationBuilder app, IWebHostEnvironment environment, IServiceProvider services)
{
    var appSettings = services.GetRequiredService<IOptions<AppSettings>>().Value;
    var swaggerSettings = services.GetRequiredService<IOptions<SwaggerSettings>>().Value;

    environment.ApplicationName = appSettings.ApplicationName;

    app.UseHttpsRedirection();
    app.UseRequestLocalization();

    app.UseRouting();
    app.UseWebOptimizer();

    app.UseWhen(context => context.IsWebRequest(), builder =>
    {
        if (!environment.IsDevelopment())
        {
            builder.UseExceptionHandler("/Errors/500");
            builder.UseHsts();
        }

        builder.UseStatusCodePagesWithReExecute("/Errors/{0}");
    });

    app.UseWhen(context => context.IsApiRequest(), builder =>
    {
        builder.UseExceptionHandler();
        builder.UseStatusCodePages();
    });

    app.UseStaticFiles();
    app.UseDefaultFiles();

    if (swaggerSettings.Enabled)
    {
        app.UseMiddleware<SwaggerAuthenticationMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{swaggerSettings.Title} {swaggerSettings.Version}");
            options.InjectStylesheet("/css/swagger.css");
        });
    }

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapEndpoints();
        endpoints.MapRazorPages();
        endpoints.MapHealthChecks("/healthchecks", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var writer = services.GetRequiredService<HealthCheckWriter>();
                await writer.WriteAsync(context, report);
            }
        });
    });
}