using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory Management API", Version = "v1" });
});

// Configure HttpClient with resilience handlers
var clientBuilder = builder.Services.AddHttpClient("FunctionClient");

clientBuilder.AddStandardResilienceHandler().Configure(options =>
{
    options.CircuitBreaker.FailureThreshold = 5;
    options.CircuitBreaker.MinimumThroughput = 10;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    options.Retry.BackoffType = Microsoft.Extensions.Http.Resilience.BackoffType.Exponential;
    options.Retry.RetryCount = 3;
    options.Retry.BaseDelay = TimeSpan.FromSeconds(2);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Management API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
