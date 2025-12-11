using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.Routing;
using MinimalHelpers.Validation;
using OperationResults.AspNetCore.Http;
using SimpleTransit;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using TinyHelpers.Json.Serialization;
using TournamentTracker.BusinessLayer.Clients;
using TournamentTracker.BusinessLayer.Clients.Interfaces;
using TournamentTracker.BusinessLayer.Notifications;
using TournamentTracker.BusinessLayer.Services;
using TournamentTracker.BusinessLayer.Settings;
using TournamentTracker.BusinessLayer.Validation;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Extensions;
using TournamentTracker.Swagger;
using ResultErrorResponseFormat = OperationResults.AspNetCore.Http.ErrorResponseFormat;
using ValidationErrorResponseFormat = MinimalHelpers.Validation.ErrorResponseFormat;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new AppSettings();
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new SwaggerSettings();

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRequestLocalization(settings.SupportedCultures);
builder.Services.AddWebOptimizer(minifyCss: true, minifyJavaScript: builder.Environment.IsProduction());

builder.Services.AddDefaultExceptionHandler();
builder.Services.AddDefaultProblemDetails();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddRequestTimeouts();

builder.Services.AddValidatorsFromAssemblyContaining<SaveTournamentRequestValidator>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
});

if (swagger.IsEnabled)
{
    builder.Services.AddOpenApi(options =>
    {
        options.RemoveServerList();
        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();
    });
}

builder.Services.AddOperationResult(options =>
{
    options.ErrorResponseFormat = ResultErrorResponseFormat.List;
});

builder.Services.ConfigureValidation(options =>
{
    options.ErrorResponseFormat = ValidationErrorResponseFormat.List;
});

builder.Services.AddDbContext<IDataContext, DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddSingleton<IEmailClient, EmailClient>();
builder.Services.AddSimpleTransit(options =>
{
    options.RegisterServicesFromAssemblyContaining<PersonEmailNotificationHandler>();
});

builder.Services.Scan(scan => scan.FromAssemblyOf<TournamentService>()
    .AddClasses(classes => classes.InNamespaceOf<TournamentService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

var app = builder.Build();
app.Environment.ApplicationName = settings.ApplicationName;

app.UseHttpsRedirection();

app.UseWhen(context => context.IsWebRequest(), builder =>
{
    if (!app.Environment.IsDevelopment())
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

app.UseWebOptimizer();
app.UseStaticFiles();

if (swagger.IsEnabled)
{
    app.UseMiddleware<SwaggerBasicAuthenticationMiddleware>();
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", settings.ApplicationName);
        options.InjectStylesheet("/css/swagger.css");
    });
}

app.UseRouting();
app.UseRequestLocalization();

app.UseWhen(context => context.IsApiRequest(), builder =>
{
    builder.UseRequestTimeouts();
});

app.MapRazorPages();
app.MapEndpoints();

app.Run();