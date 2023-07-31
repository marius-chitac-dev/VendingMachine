using Microsoft.EntityFrameworkCore;
using System.Reflection;
using webapi.Core;

namespace webapi.Infrastructure;

public class VendingMachineContext : DbContext
{
    public VendingMachineContext(DbContextOptions<VendingMachineContext> options)
       : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
