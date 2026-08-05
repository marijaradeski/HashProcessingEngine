using System.ComponentModel.DataAnnotations;

namespace HashProcessingEngine.Application.Options;
public class HashGenerationOptions
{
    public const string SectionName = "HashGeneration";

    [Range(1, int.MaxValue)]
    public int DefaultCount { get; set; }

    [Range(1, int.MaxValue)]
    public int MaximumCount { get; set; }
}
