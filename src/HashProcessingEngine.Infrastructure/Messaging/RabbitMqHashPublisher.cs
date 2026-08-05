using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Messaging;
public class RabbitMqHashPublisher : IHashPublisher
{
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqHashPublisher> _logger;

    public RabbitMqHashPublisher(IRabbitMqConnection connection, IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqHashPublisher> logger)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishAsync(HashBatchMessage message, CancellationToken cancellationToken)
    {
        await using var channel = _connection.CreateChannel();

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Publishing batch containing {Count} hashes.",message.Hashes.Count);

        var json = JsonSerializer.Serialize(message);
        var body =Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully published batch containing {Count} hashes.", message.Hashes.Count);
    }
}