using webapi.Core;

namespace webapi.FunctionalTests.Helpers.Generators;
public static class Generator
{
    public static PostUserDto CreateSeller()
    {
        return new PostUserDto("User 1", "pa$$w0rd", RoleEnum.Seller);
    }

    public static PostUserDto CreateBuyer()
    {
        return new PostUserDto("User 1", "pa$$w0rd", RoleEnum.Buyer);
    }

    public static PostDepositDto CreatePostDepositDto(int deposit)
    {
        return new PostDepositDto(deposit);
    }
}
