using HashProcessingEngine.Application.Messages;
using System.Threading.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Worker.Services;
public class HashMessageChannel
{
    private readonly Channel<HashMessage> _channel;

    public HashMessageChannel() {
        _channel = Channel.CreateUnbounded<HashMessage>();
    }

    public ChannelReader<HashMessage> Reader =>
        _channel.Reader;

    public ValueTask WriteAsync(HashMessage message, CancellationToken cancellationToken) {
        return _channel.Writer.WriteAsync(message, cancellationToken);
    }
}