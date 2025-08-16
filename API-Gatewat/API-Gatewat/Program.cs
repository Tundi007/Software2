using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Your Next.js frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

builder.Services.Configure<FormOptions>(o =>
{
    // Example: 2 GiB
    o.MultipartBodyLengthLimit = 5L * 1024 * 1024 * 1024;
});

// 2) Raise Kestrel request body size (overall)
builder.WebHost.ConfigureKestrel(o =>
{
    // Set a large limit or null to disable
    o.Limits.MaxRequestBodySize = 5L * 1024 * 1024 * 1024; // 2 GiB
                                                           // o.Limits.MaxRequestBodySize = null; // (disables Kestrel limit)
});

builder.Services.AddHealthChecks()
    // Process is up
    .AddCheck("self", () => HealthCheckResult.Healthy());
    //.AddUrlGroup(new Uri("http://localhost:5500/health"), name: "iam")
    //.AddUrlGroup(new Uri("http://localhost:5700/health"), name: "media");

var app = builder.Build();


app.UseCors("AllowFrontend");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Name == "self"
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});



app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    // If path starts with /internal-api, bypass Ocelot and return a direct response
    if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Direct response without Ocelot");
        return;
    }

    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response status: {context.Response.StatusCode}");
});

await app.UseOcelot();

app.Run();
