using System.ComponentModel.DataAnnotations;

namespace HashProcessingEngine.Application.DTOs;
public class GenerateHashesRequest
{
    [Range(1, 60000, ErrorMessage = "Count must be between 1 and 60000.")]
    public int? Count { get; set; } = 40000;
}