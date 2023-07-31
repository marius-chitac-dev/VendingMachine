using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webapi.Core;
using webapi.Core.Interfaces;
using webapi.Infrastructure;

namespace webapi.Controllers;
[Route("api")]
[ApiController]
public class VendingController : ControllerBase
{
    private readonly VendingMachineContext _context;
    private readonly IChangeCalculator _changeCalculator;

    public VendingController(VendingMachineContext context, IChangeCalculator changeCalculator)
    {
        _context = context;
        _changeCalculator = changeCalculator;
    }

    [HttpPost("deposit")]
    [Authorize(Roles = "Buyer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepositSummaryDto>> Deposit(PostDepositDto depositDto)
    {
        var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        if (user == null)
        {
            return Unauthorized();
        }
        user.IncreaseDeposit(depositDto.Deposit);
        await _context.SaveChangesAsync();

        return Ok(new DepositSummaryDto(user.Deposit));
    }

    [HttpPost("buy")]
    [Authorize(Roles = "Buyer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartSummaryDto>> Buy(CartDto cartDto)
    {
        var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        if (user == null)
        {
            return Unauthorized();
        }

        var product = await _context.Products.FindAsync(cartDto.ProductId);
        if (product == null)
        {
            return NotFound(cartDto.ProductId);
        }

        if (!product.IsAvailable(cartDto.Amount))
        {
            ModelState.AddModelError(nameof(cartDto.Amount), $"Available amount is: {product.AmountAvailable}.");
            return ValidationProblem(ModelState);
        }

        var totalCartAmount = product.Sell(cartDto.Amount);
        if (!user.HasEnoughDeposit(totalCartAmount))
        {
            if (cartDto.Amount > 1)
            {
                ModelState.AddModelError(nameof(cartDto.Amount), $"You don't have enough money for the selected amount of products.");
            }
            else
            {
                ModelState.AddModelError(nameof(cartDto.ProductId), $"You don't have enough money for the selected product.");
            }
            return ValidationProblem(ModelState);
        }

        var change = _changeCalculator.CalculateChange(user.Deposit, totalCartAmount);
        user.DecreaseDeposit(totalCartAmount + change.Sum());

        await _context.SaveChangesAsync();
        return Ok(new CartSummaryDto(totalCartAmount, new ProductBoughtDto(product.Id, cartDto.Amount, product.ProductName, product.SellerId), change));
    }

    [HttpPost("reset")]
    [Authorize(Roles = "Buyer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Reset()
    {
        var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
        if (user == null)
        {
            return Unauthorized();
        }
        user.DecreaseDeposit(user.Deposit);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
