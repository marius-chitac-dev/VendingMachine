namespace webapi.Core.Interfaces;

public interface IChangeCalculator
{
    public IEnumerable<int> CalculateChange(decimal deposit, decimal amount);
}
