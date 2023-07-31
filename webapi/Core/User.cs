using System.ComponentModel.DataAnnotations;
using webapi.Core.Validators;

namespace webapi.Core;

public class User
{
    public int Id { get; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public decimal Deposit { get; private set; }
    public RoleEnum Role { get; }

    protected User(int id, string username, string password, decimal deposit, RoleEnum role) : this(username, password, role)
    {
        Id = id;
        Deposit = deposit;
    }

    public User(string username, string password, RoleEnum role)
    {
        Username = username;
        Password = password;
        Role = role;
    }

    public User(PostUserDto createUserDto)
    {
        Username = createUserDto.Username;
        Password = createUserDto.Password;
        Role = createUserDto.Role;
    }

    public UserDto ToUserDto()
    {
        return new UserDto(Id, Username, Deposit, Role);
    }

    public void UpdateProfile(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public void IncreaseDeposit(int deposit)
    {
        Deposit += deposit;
    }

    internal void DecreaseDeposit(decimal amount)
    {
        Deposit -= amount;
    }

    public bool HasEnoughDeposit(decimal amount)
    {
        return Deposit >= amount;
    }
}

public record PostUserDto : LoginDto
{
    public PostUserDto(string username, string password, RoleEnum role) : base(username, password)
    {
        Username = username;
        Password = password;
        Role = role;
    }

    [Required]
    public RoleEnum Role { get; set; }
}

public record PutUserDto : LoginDto
{
    public PutUserDto(int id, string username, string password) : base(username, password)
    {
        Id = id;
        Username = username;
        Password = password;
    }
    [Required]
    public int Id { get; set; }
}

public record LoginDto
{
    public LoginDto(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

public record UserDto(int Id, string Username, decimal Deposit, RoleEnum Role);

public record PostDepositDto
{
    public PostDepositDto(int deposit)
    {
        Deposit = deposit;
    }
    [Required]
    [DepositValidator]
    public int Deposit { get; set; }
}

public record DepositSummaryDto(decimal Deposit);