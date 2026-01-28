using System.Text.Json;
using System.Text.Json.Serialization;
using API.Endpoints;
using API.Extensions;
using API.Middleware;
using Application.Common.Behaviors;
using Application;
using FluentValidation;
using Infrastructure;
using MediatR;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, serilogConfig) =>
    {
        serilogConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console();
    }, writeToProviders: true);

// =========================
// Service Configuration
// =========================

builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

const string OtlpEndpointConfigKey = "OpenTelemetry:Exporter:OtlpEndpoint";
var serviceName = builder.Configuration.GetValue<string>("OpenTelemetry:ServiceName") ?? builder.Environment.ApplicationName;
var otlpEndpointConfig = builder.Configuration.GetValue<string>(OtlpEndpointConfigKey) ?? "none";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracer => tracer
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(serviceName)
        .AddSource("Wolverine")
        .AddOtlpExporter(options =>
        {
            var endpoint = builder.Configuration.GetValue<string>(OtlpEndpointConfigKey);
            if (!string.IsNullOrWhiteSpace(endpoint))
                options.Endpoint = new Uri(endpoint);
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter(options =>
        {
            var endpoint = builder.Configuration.GetValue<string>(OtlpEndpointConfigKey);
            if (!string.IsNullOrWhiteSpace(endpoint))
                options.Endpoint = new Uri(endpoint);
        }));

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.ParseStateValues = true;
    logging.AddOtlpExporter(options =>
    {
        var endpoint = builder.Configuration.GetValue<string>(OtlpEndpointConfigKey);
        if (!string.IsNullOrWhiteSpace(endpoint))
            options.Endpoint = new Uri(endpoint);
    });
});

// ASP.NET Core Services
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

// Pipeline Behaviors
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// MediatR + FluentValidation
builder.Services.AddMediatR(config =>
    config.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

builder.Services.AddValidatorsFromAssembly(
    typeof(ApplicationAssemblyMarker).Assembly);

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddHttpClient("default")
    .AddPolicyHandler(ResilienceExtensions.CreateRetryPolicy(builder.Configuration))
    .AddPolicyHandler(ResilienceExtensions.CreateCircuitBreakerPolicy(builder.Configuration))
    .AddPolicyHandler(ResilienceExtensions.CreateTimeoutPolicy(builder.Configuration));

// Wolverine (Messaging) configuration
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    opts.ConfigureWolverine(builder.Configuration);
});

builder.Services.AddHealthChecks();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

Log.Information(
    "Starting {ServiceName} in {Environment} with OTLP endpoint {OtlpEndpoint} and URLs {Urls}",
    serviceName,
    app.Environment.EnvironmentName,
    otlpEndpointConfig,
    builder.Configuration["ASPNETCORE_URLS"] ?? "defaults");

// Add global exception handler for FluentValidation exceptions
app.UseFluentValidationExceptionHandler();

// HTTP Request Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Evaluation Service 1 API");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");

app.MapEvaluationEndpoints();

app.MapHealthChecks("/health");

app.Run();

/// <summary>
/// Program class for the API application.
/// Exposed as partial for integration testing via WebApplicationFactory.
/// </summary>
public partial class Program { }
