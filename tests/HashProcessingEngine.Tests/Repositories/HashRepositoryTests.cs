using Dapper;
using HashProcessingEngine.Application.Messages;
using HashProcessingEngine.Infrastructure.Persistence.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Tests.Repositories;

[TestFixture]
public class HashRepositoryTests
{
    private HashRepository _repository = null!;

    private string _connectionString ="Server=localhost;Port=3306;Database=hashprocessing;User=hashuser;Password=HashPassword123!;";


    [SetUp]
    public async Task Setup()
    {
        _repository = new HashRepository(_connectionString);

        await ClearDatabase();
    }

    private async Task ClearDatabase()
    {
        await using var connection =new MySqlConnection(_connectionString);

        await connection.ExecuteAsync("DELETE FROM hashes;");
    }

    [Test]
    public async Task GetAllAsyncGroupHashesByDate()
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        await _repository.SaveAsync(
            new HashMessage
            {
                Value = "hash1",
                CreatedAt = yesterday
            },
            CancellationToken.None);

        await _repository.SaveAsync(
            new HashMessage
            {
                Value = "hash2",
                CreatedAt = yesterday
            },
            CancellationToken.None);

        await _repository.SaveAsync(
            new HashMessage
            {
                Value = "hash3",
                CreatedAt = today
            },
            CancellationToken.None);

        var result = await _repository.GetAllAsync(CancellationToken.None);

        Assert.That(result, Is.Not.Null);

        var yesterdayResult = result.Hashes.First(x => x.Date == yesterday.ToString("yyyy-MM-dd"));
        var todayResult = result.Hashes.First(x => x.Date == today.ToString("yyyy-MM-dd"));

        Assert.That(yesterdayResult.Count, Is.EqualTo(2));
        Assert.That(todayResult.Count, Is.EqualTo(1));
    }
}