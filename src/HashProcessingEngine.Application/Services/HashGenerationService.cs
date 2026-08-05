using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace HashProcessingEngine.Application.Services;
public class HashGenerationService : IHashProcessingService
{
    private readonly IHashGenerator _hashGenerator;
    private readonly IHashPublisher _hashPublisher;

    private readonly HashGenerationOptions _options;
    private readonly HashBatchOptions _batchOptions;

    public HashGenerationService(IHashGenerator hashGenerator, IHashPublisher hashPublisher,
        IOptions<HashGenerationOptions> options,
        IOptions<HashBatchOptions> batchOptions)
    {
        _hashGenerator = hashGenerator;
        _hashPublisher = hashPublisher;

        _options = options.Value;
        _batchOptions = batchOptions.Value;
    }

    public async Task<GenerateHashesResponse> GenerateAsync(GenerateHashesRequest request, CancellationToken cancellationToken)
    {
        var count = request.Count;

        if (count > _options.MaximumCount)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Count),
                $"Maximum allowed count is {_options.MaximumCount}");
        }

        var hashes = new List<HashMessage>();

        for (var i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            hashes.Add(
                new HashMessage
                {
                    Value = _hashGenerator.Generate(),
                    CreatedAt = DateTime.UtcNow
                });
        }

        var batches = hashes
            .Chunk(_batchOptions.BatchSize)
            .Select(x => new HashBatchMessage
            {
                Hashes = x.ToList()
            })
            .ToList();

        using var semaphore = new SemaphoreSlim(_batchOptions.ParallelPublishers);

        var publishTasks = batches.Select(async batch =>
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                await _hashPublisher.PublishAsync(batch, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(publishTasks);

        return new GenerateHashesResponse
        {
            Requested = count,
            Generated = hashes.Count,
            Message = "Hash generation completed."
        };
    }
}