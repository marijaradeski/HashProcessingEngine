using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Messages;

namespace HashProcessingEngine.Application.Interfaces;
public interface IHashRepository
{
    Task SaveAsync(HashMessage message, CancellationToken cancellationToken);

    Task<HashCountResponse> GetAllAsync(CancellationToken cancellationToken);
}