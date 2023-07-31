using System.ComponentModel.DataAnnotations;

namespace webapi.Core;

public class Product
{
    public int Id { get; }
    public int AmountAvailable { get; private set; }
    public decimal Cost { get; private set; }
    public string ProductName { get; private set; }
    public int SellerId { get; }

    protected Product(int id, int amountAvailable, decimal cost, string productName, int sellerId) : this(amountAvailable, cost, productName, sellerId)
    {
        Id = id;
    }

    public Product(int amountAvailable, decimal cost, string productName, int sellerId)
    {
        AmountAvailable = amountAvailable;
        Cost = cost;
        ProductName = productName;
        SellerId = sellerId;
    }

    public ProductDto ToProductDto()
    {
        return new ProductDto(Id, AmountAvailable, Cost, ProductName, SellerId);
    }

    public void Update(int amountAvailable, decimal cost, string productName)
    {
        AmountAvailable = amountAvailable;
        Cost = cost;
        ProductName = productName;
    }

    public bool IsAvailable(int amount)
    {
        return AmountAvailable >= amount;
    }

    public decimal Sell(int amount)
    {
        AmountAvailable -= amount;
        return Cost * amount;
    }
}

public record PostProductDto
{
    public PostProductDto(int amountAvailable, decimal cost, string productName)
    {
        AmountAvailable = amountAvailable;
        Cost = cost;
        ProductName = productName;
    }

    [Required]
    [Range(0, int.MaxValue)]
    public int AmountAvailable { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Cost { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; }
}

public record PutProductDto : PostProductDto
{
    public PutProductDto(int amountAvailable, decimal cost, string productName) : base(amountAvailable, cost, productName)
    {
    }
    [Required]
    public int Id { get; set; }
}

public record ProductDto(int Id, int AmountAvailable, decimal Cost, string ProductName, int SellerId);

public record CartDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }
}

public record ProductBoughtDto(int Id, int Amount, string ProductName, int SellerId);

public record CartSummaryDto(decimal TotalSpent, ProductBoughtDto Product, IEnumerable<int> Change);