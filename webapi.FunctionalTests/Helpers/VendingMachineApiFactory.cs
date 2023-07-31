using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using System.Net.Http.Headers;
using Testcontainers.MsSql;
using webapi.Infrastructure;

namespace webapi.FunctionalTests.Helpers;
public class VendingMachineApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = default!;

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<VendingMachineContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<VendingMachineContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });

            services.AddAuthentication("TestScheme")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("TestScheme", null);
        });
        return base.CreateHost(builder);
    }

    public Task ResetDatabaseAsync() => _respawner.ResetAsync(_dbConnection);


    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());
        _dbConnection.Open();
        HttpClient = CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    public async Task InitializeRespawner()
    {
        if (_respawner == default)
        {
            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = new[] { "dbo" },
                TablesToInclude = new Table[] { "Products", "Users" },
                TablesToIgnore = new Table[] { "__EFMigrationsHistory", }
            });
        }
    }

    public new async Task DisposeAsync()
    {
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}
