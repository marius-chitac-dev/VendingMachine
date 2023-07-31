using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.Core;

namespace webapi.Infrastructure.EntityConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Deposit)
            .HasPrecision(18, 2)
            .HasDefaultValueSql("((0))");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany<Product>()
            .WithOne()
            .HasForeignKey(p => p.SellerId)
            .IsRequired();
    }
}
