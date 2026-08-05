namespace HashProcessingEngine.Application.DTOs;
public class GenerateHashesResponse
{
    public int Requested { get; set; }

    public int Generated { get; set; }

    public string Message { get; set; } = string.Empty;
}