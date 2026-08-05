using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;

namespace HashProcessingEngine.Application.Services;
public class HashQueryService : IHashQueryService
{
    private readonly IHashRepository _repository;

    public HashQueryService(IHashRepository repository)
    {
        _repository = repository;
    }

    public async Task<HashCountResponse> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}