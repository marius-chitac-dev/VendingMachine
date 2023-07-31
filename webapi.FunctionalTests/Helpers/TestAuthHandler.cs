using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace webapi.FunctionalTests.Helpers;
public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public int DefaultUserId { get; set; } = 1;
}
public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string UserId = "UserId";
    public const string Role = "Role";
    private readonly int _defaultUserId;

    public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _defaultUserId = options.CurrentValue.DefaultUserId;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new Claim[2];
        if (Context.Request.Headers.TryGetValue(UserId, out var userId))
        {
            if (int.Parse(userId[0]!) == 0)
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthenticated!"));
            }
            claims[0] = new Claim(ClaimTypes.NameIdentifier, userId[0]!);
        }
        else
        {
            claims[0] = new Claim(ClaimTypes.NameIdentifier, _defaultUserId.ToString()!);
        }
        Context.Request.Headers.TryGetValue(Role, out var role);
        if (!string.IsNullOrEmpty(role))
        {
            claims[1] = new Claim(ClaimTypes.Role, role!);
        }
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");
        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}