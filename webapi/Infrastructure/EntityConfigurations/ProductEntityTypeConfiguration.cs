using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.Core;

namespace webapi.Infrastructure.EntityConfigurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.AmountAvailable)
            .IsRequired();

        builder.Property(p => p.Cost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.ProductName)
            .HasMaxLength(100)
            .IsRequired();
    }
}

