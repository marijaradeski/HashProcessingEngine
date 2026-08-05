using RabbitMQ.Client;
using HashProcessingEngine.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Messaging;
public class RabbitMqConnection : IRabbitMqConnection
{
    private readonly IConnection _connection;

    public RabbitMqConnection(ConnectionFactory factory) {
        _connection = factory.CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();
    }

    public IChannel CreateChannel() {
        return _connection.CreateChannelAsync()
            .GetAwaiter()
            .GetResult();
    }
}