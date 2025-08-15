using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using StorageService.Application;
using StorageService.Application.Interfaces;
using StorageService.Infrastructure;
using StorageService.Infrastructure.Context;
using StorageService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// For MySQL 8.0 (server version is important for Pomelo)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

builder.Services.AddDbContext<SQLServerContext>(options =>
    options.UseMySql(cs, serverVersion));

builder.Services.AddInfrastructure().AddApplication();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpClient("iam", client =>
{
    client.BaseAddress = new Uri("http://iam:8080/"); // change to actual IAM service
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var storage = scope.ServiceProvider.GetRequiredService<ICloudStorageService>();
        /*await storage.EnsureBucketExistsAsync();*/ // or skip for user-specific buckets
    }
    catch (Exception ex)
    {
        Console.WriteLine("Startup error: " + ex.Message);
        // Optionally rethrow or log
    }
}


if (app.Environment.IsEnvironment("Container"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<StorageService.Infrastructure.Context.SQLServerContext>();
    await db.Database.MigrateAsync();
}


// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
    }
    else
    {
        await next();
    }
});

app.UseAuthorization();


app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
