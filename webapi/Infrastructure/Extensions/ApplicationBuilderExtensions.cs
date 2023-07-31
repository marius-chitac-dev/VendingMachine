using Microsoft.EntityFrameworkCore;

namespace webapi.Infrastructure.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void EnsureDatabaseMigration(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<VendingMachineContext>();
        context.Database.Migrate();
    }
}
