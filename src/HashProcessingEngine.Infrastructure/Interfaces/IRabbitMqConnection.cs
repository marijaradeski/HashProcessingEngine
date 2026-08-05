using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Interfaces;
public interface IRabbitMqConnection
{
    IChannel CreateChannel();
}