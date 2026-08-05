using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Worker.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Worker.Services;
public class HashWorkerService : BackgroundService
{
    private readonly HashMessageChannel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WorkerOptions _options;
    private readonly ILogger<HashWorkerService> _logger;

    public HashWorkerService(HashMessageChannel channel, IServiceScopeFactory scopeFactory,
        IOptions<WorkerOptions> options,
        ILogger<HashWorkerService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {

        _logger.LogInformation("Starting {Count} hash workers.", _options.WorkerCount);
 
        var workers = new List<Task>();

        for (var i = 0; i < _options.WorkerCount; i++)
        {
            var workerId = i + 1;

            workers.Add(ProcessMessagesAsync(workerId, stoppingToken));
        }

        await Task.WhenAll(workers);
    }

    private async Task ProcessMessagesAsync(int workerId, CancellationToken cancellationToken) {
        _logger.LogInformation("Worker {WorkerId} started.", workerId);

        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken)) {
              
            try {
                using var scope = _scopeFactory.CreateScope();

                var repository = scope.ServiceProvider.GetRequiredService<IHashRepository>();

                await repository.SaveAsync(message, cancellationToken);

                _logger.LogInformation("Worker {WorkerId} saved hash {Hash}.", workerId, message.Value);
            }
            catch (Exception ex) {
                _logger.LogError(ex,"Worker {WorkerId} failed.", workerId);
            }
        }
    }
}