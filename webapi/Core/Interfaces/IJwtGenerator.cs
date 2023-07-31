namespace webapi.Core.Interfaces;

public interface IJwtGenerator
{
    public string CreateToken(User user);
}
