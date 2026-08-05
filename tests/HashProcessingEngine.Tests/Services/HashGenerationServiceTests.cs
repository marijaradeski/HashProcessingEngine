using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Application.Services;
using HashProcessingEngine.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Tests.Services;

public class HashGenerationServiceTests
{
    private readonly Mock<IHashGenerator> _hashGenerator;
    private readonly Mock<IHashPublisher> _publisher;

    private readonly HashGenerationService _service;

    public HashGenerationServiceTests()
    {
        _hashGenerator = new Mock<IHashGenerator>();
        _publisher = new Mock<IHashPublisher>();

        _hashGenerator
            .Setup(x => x.Generate())
            .Returns("testhash");

        _service = new HashGenerationService(
            _hashGenerator.Object,
            _publisher.Object,
            Options.Create(new HashGenerationOptions
            {
                DefaultCount = 10,
                MaximumCount = 1000
            }),
            Options.Create(new HashBatchOptions
            {
                BatchSize = 5,
                ParallelPublishers = 2
            }));
    }

    [Test]
    public async Task GenerateAsyncGenerateAndPublishBatches()
    {
        var request = new GenerateHashesRequest
        {
            Count = 10
        };

        var result =
            await _service.GenerateAsync(
                request,
                CancellationToken.None);

        Assert.That(result.Generated, Is.EqualTo(10));

        _publisher.Verify(
            x => x.PublishAsync(
                It.IsAny<HashBatchMessage>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Test]
    public async Task GenerateAsyncPublishCorrectBatchSize()
    {
        HashBatchMessage? capturedBatch = null;

        _publisher
            .Setup(x => x.PublishAsync(It.IsAny<HashBatchMessage>(), It.IsAny<CancellationToken>()))
            .Callback<HashBatchMessage, CancellationToken>(
                (batch, token) =>
                {
                    capturedBatch = batch;
                })
            .Returns(Task.CompletedTask);

        await _service.GenerateAsync(
            new GenerateHashesRequest
            {
                Count = 3
            },
            CancellationToken.None);

        Assert.That(capturedBatch != null);
        Assert.That(capturedBatch!.Hashes.Count, Is.EqualTo(3));
    }
}