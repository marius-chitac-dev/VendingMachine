using System.ComponentModel.DataAnnotations;

namespace webapi.Infrastructure.Options;

public class VendingOptions
{
    public const string Name = "Vending";

    [Required]
    [MinLength(1)]
    public IEnumerable<int> AcceptedCoins { get; set; } = default!;
}