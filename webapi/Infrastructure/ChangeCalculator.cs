using Microsoft.Extensions.Options;
using webapi.Core.Interfaces;
using webapi.Infrastructure.Options;

namespace webapi.Infrastructure;

public class ChangeCalculator : IChangeCalculator
{
    private readonly List<int> _acceptedCoins;

    public ChangeCalculator(IOptions<VendingOptions> options)
    {
        _acceptedCoins = options.Value.AcceptedCoins.Distinct().OrderDescending().ToList();
    }

    public IEnumerable<int> CalculateChange(decimal deposit, decimal amount)
    {
        var change = deposit - amount;
        IEnumerable<int> result = new List<int>();

        foreach (var coin in _acceptedCoins)
        {
            while (change > coin)
            {
                result = result.Append(coin);
                change -= coin;
            }
        }
        return result;
    }
}
