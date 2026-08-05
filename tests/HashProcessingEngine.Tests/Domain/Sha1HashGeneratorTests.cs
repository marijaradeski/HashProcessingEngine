using HashProcessingEngine.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Tests.Domain;

[TestFixture]
public class Sha1HashGeneratorTests
{
    private Sha1HashGenerator _generator = null!;

    [SetUp]
    public void Setup()
    {
        _generator = new Sha1HashGenerator();
    }

    [Test]
    public void GenerateReturnsValidSha1Hash()
    {
        // Act
        var result = _generator.Generate();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.EqualTo(40));
        Assert.That(result, Does.Match("^[a-fA-F0-9]{40}$"));
    }
}