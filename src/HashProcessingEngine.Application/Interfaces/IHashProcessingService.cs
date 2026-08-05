using HashProcessingEngine.Application.DTOs;

namespace HashProcessingEngine.Application.Interfaces;
public interface IHashProcessingService
{
    Task<GenerateHashesResponse> GenerateAsync(GenerateHashesRequest request,CancellationToken cancellationToken);
}