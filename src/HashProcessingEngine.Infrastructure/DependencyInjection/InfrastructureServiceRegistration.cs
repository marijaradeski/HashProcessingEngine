using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Domain.Interfaces;
using HashProcessingEngine.Infrastructure.Interfaces;
using HashProcessingEngine.Infrastructure.Messaging;
using HashProcessingEngine.Infrastructure.Persistence.Repositories;
using HashProcessingEngine.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.DependencyInjection;
public static class InfrastructureServiceRegistration
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {

        /* Domain services */
        services.AddSingleton<IHashGenerator,Sha1HashGenerator>();

        /* Database */
        services.AddScoped<IHashRepository>(provider =>
        {
            var connectionString = configuration.GetConnectionString("MariaDb");

            return new HashRepository(connectionString!);
        });

        /* RabbitMQ connection */
        services.AddSingleton<ConnectionFactory>(provider =>
        {

            var options = provider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>()
                .Value;

            return new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password
            };

        });

        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

        /* Messaging */
        services.AddSingleton<IHashPublisher, RabbitMqHashPublisher>();

        // RabbitMQ message consumption is handled by HashProcessingEngine.Worker.
        // The API only publishes generated hash batches.
        //services.AddHostedService<RabbitMqHashConsumer>();

        return services;
    }
}