using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MinimalHelpers.Routing;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using TinyHelpers.Json.Serialization;
using TournamentTracker.BusinessLayer.Settings;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Extensions;
using TournamentTracker.Swagger;

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

if (swagger.IsEnabled)
{
    builder.Services.AddOpenApi(options =>
    {
        options.RemoveServerList();
        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();
    });
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
});

builder.Services.AddDbContext<IDataContext, DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
    options.UseSqlServer(connectionString);
});

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

app.MapRazorPages();
app.MapEndpoints();

app.Run();