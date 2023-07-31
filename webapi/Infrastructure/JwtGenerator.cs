using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Core;
using webapi.Core.Interfaces;
using webapi.Infrastructure.Options;
namespace webapi.Infrastructure;

public class JwtGenerator : IJwtGenerator
{
    private readonly string _key;

    public JwtGenerator(IOptions<AppSettingsOptions> options)
    {
        _key = options.Value.SigningKey;
    }

    public string CreateToken(User user)
    {
        List<Claim> claims = new() {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
