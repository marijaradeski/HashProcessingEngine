using System.Text.Json;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Infrastructure.Interfaces;
using HashProcessingEngine.Worker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Worker.Consumers;
public class RabbitMqHashConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly HashMessageChannel _channel;
    private readonly ILogger<RabbitMqHashConsumer> _logger;

    public RabbitMqHashConsumer(IRabbitMqConnection connection, IOptions<RabbitMqOptions> options,
        HashMessageChannel channel,
        ILogger<RabbitMqHashConsumer> logger)
    {
        _connection = connection;
        _options = options.Value;
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await using var channel = _connection.CreateChannel();

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) => {
            try {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var batch = JsonSerializer.Deserialize<HashBatchMessage>(json);

                if (batch is not null) {
                    foreach (var hash in batch.Hashes) {
                        await _channel.WriteAsync(hash, stoppingToken);
                    }
                }

                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    false,
                    stoppingToken);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error processing RabbitMQ message.");
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("RabbitMQ consumer started.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}