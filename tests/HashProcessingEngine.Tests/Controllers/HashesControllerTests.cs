using HashProcessingEngine.Api.Controllers;
using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Tests.Controllers;

[TestFixture]
public class HashesControllerTests
{
    private Mock<IHashProcessingService> _processingServiceMock = null!;
    private Mock<IHashQueryService> _queryServiceMock = null!;

    private HashesController _controller = null!;


    [SetUp]
    public void Setup()
    {
        _processingServiceMock = new Mock<IHashProcessingService>();
        _queryServiceMock = new Mock<IHashQueryService>();

        _controller = new HashesController(_processingServiceMock.Object, _queryServiceMock.Object);
    }

    [Test]
    public async Task PostReturnsResultWhenHashesAreGenerated()
    {
        // Arrange
        var response = new GenerateHashesResponse
        {
            Requested = 5,
            Generated = 5,
            Message = "Hash generation completed."
        };

        _processingServiceMock
            .Setup(x => x.GenerateAsync(
                It.IsAny<GenerateHashesRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Generate(
            new GenerateHashesRequest
            {
                Count = 5
            },
            CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);

        _processingServiceMock.Verify(
            x => x.GenerateAsync(
                It.IsAny<GenerateHashesRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetReturnsResultWhenHashesExist()
    {
        // Arrange
        var response = new HashCountResponse
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

        _queryServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);

        _queryServiceMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
