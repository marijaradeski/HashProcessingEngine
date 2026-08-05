using Dapper;
using HashProcessingEngine.Application.DTOs;
using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Messages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Persistence.Repositories;
public class HashRepository : IHashRepository
{
    private readonly string _connectionString;

    public HashRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SaveAsync(HashMessage message, CancellationToken cancellationToken)
    {
        const string sql =
        """
        INSERT INTO hashes
        (
            Value,
            Created_At
        )
        VALUES
        (
            @Value,
            @CreatedAt
        );
        """;

        await using var connection = new MySqlConnection(_connectionString);

        var command = new CommandDefinition(
            sql,
            new
            {
                message.Value,
                message.CreatedAt
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task<HashCountResponse> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql =
        """
        SELECT
            DATE_FORMAT(Created_At, '%Y-%m-%d') AS Date,
            COUNT(*) AS Count
        FROM hashes
        GROUP BY DATE(Created_At)
        ORDER BY Date;
        """;

        await using var connection = new MySqlConnection(_connectionString);
        var result = await connection.QueryAsync<HashCountItem>(
            new CommandDefinition(
            sql,
            cancellationToken: cancellationToken));

        return new HashCountResponse
        {
            Hashes = result.ToList()
        };
    }
}