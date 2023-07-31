using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using webapi.Core;
using webapi.FunctionalTests.Helpers;
using webapi.FunctionalTests.Helpers.Generators;

namespace webapi.FunctionalTests.Endpoints;
[Collection("SharedTestCollection")]
public class DepositShould : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly Func<Task> _initializeRespawner;
    private readonly Func<Task> _resetDatabase;

    public DepositShould(VendingMachineApiFactory factory)
    {
        _httpClient = factory.HttpClient;
        _initializeRespawner = factory.InitializeRespawner;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task AcceptOnlyCertainCoins(int deposit)
    {
        var createUserResponse = await _httpClient.PostAsJsonAsync("/api/users", Generator.CreateBuyer());
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var createUserResponseContent = (await createUserResponse.Content.ReadFromJsonAsync<UserDto>(options))!;

        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.Role, "Buyer");
        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.UserId, createUserResponseContent.Id.ToString());
        var addDepositResult = await _httpClient.PostAsJsonAsync("/api/deposit", Generator.CreatePostDepositDto(deposit));
        Assert.Equal(HttpStatusCode.OK, addDepositResult.StatusCode);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.Role);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.UserId);

        var addDepositResultContent = (await addDepositResult.Content.ReadFromJsonAsync<DepositSummaryDto>())!;
        Assert.Equal(deposit, addDepositResultContent.Deposit);
    }

    [Fact]
    public async Task IncreaseDeposit()
    {
        var createUserResponse = await _httpClient.PostAsJsonAsync("/api/users", Generator.CreateBuyer());
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var createUserResponseContent = (await createUserResponse.Content.ReadFromJsonAsync<UserDto>(options))!;

        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.Role, "Buyer");
        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.UserId, createUserResponseContent.Id.ToString());
        var addDepositResult = await _httpClient.PostAsJsonAsync("/api/deposit", Generator.CreatePostDepositDto(10));
        Assert.Equal(HttpStatusCode.OK, addDepositResult.StatusCode);

        addDepositResult = await _httpClient.PostAsJsonAsync("/api/deposit", Generator.CreatePostDepositDto(10));
        Assert.Equal(HttpStatusCode.OK, addDepositResult.StatusCode);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.Role);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.UserId);

        var addDepositResultContent = (await addDepositResult.Content.ReadFromJsonAsync<DepositSummaryDto>())!;
        Assert.Equal(20, addDepositResultContent.Deposit);
    }

    [Fact]
    public async Task ReceiveValidationError()
    {
        var createUserResponse = await _httpClient.PostAsJsonAsync("/api/users", Generator.CreateBuyer());
        Assert.Equal(HttpStatusCode.Created, createUserResponse.StatusCode);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var createUserResponseContent = (await createUserResponse.Content.ReadFromJsonAsync<UserDto>(options))!;

        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.Role, "Buyer");
        _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.UserId, createUserResponseContent.Id.ToString());
        var addDepositResult = await _httpClient.PostAsJsonAsync("/api/deposit", Generator.CreatePostDepositDto(2));
        Assert.Equal(HttpStatusCode.BadRequest, addDepositResult.StatusCode);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.Role);
        _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.UserId);

        var errors = await addDepositResult.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(errors);
        Assert.Single(errors.Errors);
        Assert.Equal("Deposit", errors.Errors.First().Key);
        Assert.Single(errors.Errors.First().Value);
        Assert.Equal("You can only deposit 5, 10, 20, 50 and 100 cent coins.", errors.Errors.First().Value.First());
    }

    public Task InitializeAsync() => _initializeRespawner();

    public Task DisposeAsync() => _resetDatabase();
}
