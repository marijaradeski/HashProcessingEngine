using System.ComponentModel.DataAnnotations;


namespace HashProcessingEngine.Application.Options;
public class RabbitMqOptions
{

    public const string SectionName = "RabbitMq";

    [Required]
    public string HostName { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; }

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string QueueName { get; set; } = string.Empty;

}