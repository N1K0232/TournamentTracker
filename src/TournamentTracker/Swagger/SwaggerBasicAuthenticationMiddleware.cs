﻿using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TinyHelpers.Extensions;
using TournamentTracker.BusinessLayer.Settings;
using TournamentTracker.Extensions;

namespace TournamentTracker.Swagger;

public class SwaggerBasicAuthenticationMiddleware
{
    private readonly RequestDelegate next;
    private readonly SwaggerSettings swagger;

    public SwaggerBasicAuthenticationMiddleware(RequestDelegate next, IOptions<SwaggerSettings> swaggerOptions)
    {
        this.next = next;
        swagger = swaggerOptions.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.IsSwaggerRequest() && swagger.UserName.HasValue() && swagger.Password.HasValue())
        {
            string authenticationHeader = httpContext.Request.Headers[HeaderNames.Authorization];
            if (authenticationHeader?.StartsWith("Basic ") ?? false)
            {
                var header = AuthenticationHeaderValue.Parse(authenticationHeader);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter)).Split(':', count: 2);

                var userName = credentials.ElementAtOrDefault(0);
                var password = credentials.ElementAtOrDefault(1);

                if (userName == swagger.UserName && password == swagger.Password)
                {
                    await next.Invoke(httpContext);
                    return;
                }
            }

            httpContext.Response.Headers.WWWAuthenticate = new StringValues("Basic");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            await next.Invoke(httpContext);
        }
    }
}