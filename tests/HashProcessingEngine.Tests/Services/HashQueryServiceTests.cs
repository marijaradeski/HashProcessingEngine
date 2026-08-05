using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Tests.Services;

[TestFixture]
public class HashQueryServiceTests
{
    private Mock<IHashRepository> _repositoryMock = null!;

    private HashQueryService _service = null!;


    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IHashRepository>();

        _service = new HashQueryService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetAllAsyncReturnHashesFromRepository()
    {
        // Arrange
        var expectedResponse = new HashCountResponse
        {
            Hashes =
            [
                new HashCountItem
                {
                    Date = "2026-08-03",
                    Count = 40000
                }
            ]
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Hashes.Count, Is.EqualTo(1));
        Assert.That(result.Hashes[0].Count, Is.EqualTo(40000));

        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}