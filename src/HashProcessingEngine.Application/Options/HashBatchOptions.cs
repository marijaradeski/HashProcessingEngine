using System.ComponentModel.DataAnnotations;

namespace HashProcessingEngine.Application.Options;
public class HashBatchOptions
{
    public const string SectionName = "HashBatch";

    [Range(1, int.MaxValue)]
    public int BatchSize { get; set; } = 100;

    [Range(1, int.MaxValue)]
    public int ParallelPublishers { get; set; } = 4;
}