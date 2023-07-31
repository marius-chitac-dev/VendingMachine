using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using webapi.Infrastructure.Options;

namespace webapi.Core.Validators;
public class DepositValidator : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var deposit = (int)value!;
        var vendingOptions = (IOptions<VendingOptions>)validationContext.GetRequiredService(typeof(IOptions<VendingOptions>));

        if (vendingOptions.Value.AcceptedCoins.Contains(deposit))
        {
            return ValidationResult.Success!;
        }

        var acceptedCoins = string.Join(", ", vendingOptions.Value.AcceptedCoins);
        var lastComma = acceptedCoins.LastIndexOf(',');
        if (lastComma != -1) acceptedCoins = acceptedCoins.Remove(lastComma, 1).Insert(lastComma, " and");
        return new ValidationResult($"You can only deposit {acceptedCoins} cent coins.");
    }
}