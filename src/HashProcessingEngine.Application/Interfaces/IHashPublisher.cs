using HashProcessingEngine.Application.Messages;

namespace HashProcessingEngine.Application.Interfaces;
public interface IHashPublisher
{
    Task PublishAsync(HashBatchMessage message, CancellationToken cancellationToken);
}