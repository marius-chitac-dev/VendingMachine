using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Core;
using webapi.Core.Interfaces;
using webapi.Infrastructure;

namespace webapi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly VendingMachineContext _context;
    private readonly IJwtGenerator _jwtGenerator;

    public AuthController(VendingMachineContext context, IJwtGenerator jwtGenerator)
    {
        _context = context;
        _jwtGenerator = jwtGenerator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        var user = await _context.Users.Where(x => x.Username == loginDto.Username).FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        PasswordHasher<string> pw = new();
        var result = pw.VerifyHashedPassword(user.Username, user.Password, loginDto.Password);

        if (result != PasswordVerificationResult.Failed)
        {
            return Ok(_jwtGenerator.CreateToken(user));
        }
        return Unauthorized();
    }

}
