using Application.Common.Interfaces;
using Application.Contracts;
using Infrastructure.Messaging;
using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

namespace Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddScoped<IIntegrationEventPublisher, WolverineIntegrationEventPublisher>();

        var connectionString = config.GetConnectionString("Marten");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddMarten(opts =>
                {
                    opts.Connection(connectionString);
                    if (environment.IsDevelopment())
                    {
                        opts.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                    }
                })
                .IntegrateWithWolverine();
        }

        return services;
    }

    /// <summary>
    /// Configures Wolverine options, including RabbitMQ transport.
    /// </summary>
    public static void ConfigureWolverine(this WolverineOptions opts, IConfiguration config)
    {
        var rabbitMqConnectionString = config.GetConnectionString("RabbitMq");
        if (!string.IsNullOrWhiteSpace(rabbitMqConnectionString))
        {
            opts.UseRabbitMq(new Uri(rabbitMqConnectionString))
                .AutoProvision();

            opts.PublishMessage<PingRequested>()
                .ToRabbitQueue("eval.ping.requested");

            opts.ListenToRabbitQueue("eval.pong.returned");

            // Config for Quests
            opts.PublishMessage<DungeonCompleted>()
                .ToRabbitQueue("quests.dungeon-completed");

            opts.ListenToRabbitQueue("quests.dungeon-completed")
                .UseDurableInbox(); // Ensure Idempotency explicitly
        }

        // TODO: add saga persistence (e.g., Marten) when a store is chosen.
    }
}
