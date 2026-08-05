using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Domain.Entities;
public class GeneratedHash
{
    public Guid Id { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public GeneratedHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Hash value cannot be empty", nameof(value));
        }

        Id = Guid.NewGuid();
        Value = value;
        CreatedAt = DateTime.Now;
    }
}