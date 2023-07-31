using System.ComponentModel.DataAnnotations;

namespace webapi.Infrastructure.Options;

public class AppSettingsOptions
{
    public const string Name = "AppSettings";
    [Required]
    [MinLength(64)]
    public string SigningKey { get; set; } = default!;
}
