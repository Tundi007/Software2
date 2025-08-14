using IAM.Application;
using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.Repositories;
using IAM.Application.Common.Code;
using IAM.Application.Common.Hash;
using IAM.Application.Common.Repositories;
using IAM.Application.Common.Tokens;
using IAM.Infrastructure;
using IAM.Infrastructure.Code;
using IAM.Infrastructure.context;
using IAM.Infrastructure.DistrebutedCaching.IAM.Infrastructure.DistrebutedCaching;
using IAM.Infrastructure.Hash;
using IAM.Infrastructure.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

var redisConfig = builder.Configuration["Redis:Configuration"]
                 ?? builder.Configuration["Redis__Configuration"]  // also allow env var style
                 ?? "redis:6379"; // fallback for containers

var options = ConfigurationOptions.Parse(redisConfig, true);
options.AbortOnConnectFail = false;   // keep retrying if Redis starts slowly

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options));

builder.Services.AddScoped<ICachingContext, RedisContext>();


var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// For MySQL 8.0 (server version is important for Pomelo)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

builder.Services.AddDbContext<SQLServerDataContext>(options =>
    options.UseMySql(cs, serverVersion));

// Add services to the container.
builder.Services.AddApplication().AddInfrastructure();
//var c = ConnectionMultiplexer.Connect("127.0.0.1:2028");
//builder.Services.AddSingleton<IConnectionMultiplexer>(c);





builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Logging.AddConsole();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

if (app.Environment.IsEnvironment("Container"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<IAM.Infrastructure.context.SQLServerDataContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");


app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.UseHealthChecks("/health");

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

app.Run();
