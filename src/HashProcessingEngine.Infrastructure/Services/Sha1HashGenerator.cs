using System.Security.Cryptography;
using HashProcessingEngine.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Infrastructure.Services;
public class Sha1HashGenerator : IHashGenerator
{
    public string Generate()
    {
        var randomData = Guid.NewGuid().ToString();

        using var sha1 = SHA1.Create();

        var bytes = Encoding.UTF8.GetBytes(randomData);
        var hashBytes = sha1.ComputeHash(bytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}