using System.Text.Json;
using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Messaging.Consumers;
public class RabbitMqHashConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqHashConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqHashConsumer(IRabbitMqConnection rabbitMqConnection, IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqHashConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _options = options.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var channel =_rabbitMqConnection.CreateChannel();

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var batch = JsonSerializer.Deserialize<HashBatchMessage>(json);

                if (batch is null)
                {
                    _logger.LogWarning("Invalid hash batch received.");

                    await channel.BasicNackAsync(eventArgs.DeliveryTag ,false, false);

                    return;
                }

                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider
                    .GetRequiredService<IHashRepository>();

                foreach (var hash in batch.Hashes)
                {
                    await repository.SaveAsync(hash, stoppingToken);
                }

                await channel.BasicAckAsync(eventArgs.DeliveryTag, false);

                _logger.LogInformation("Hash batch saved successfully. Count: {Count}", batch.Hashes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message.");

                await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("RabbitMQ consumer started.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}