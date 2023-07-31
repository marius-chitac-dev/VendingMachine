using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webapi.Core;
using webapi.Infrastructure;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly VendingMachineContext _context;

        public UsersController(VendingMachineContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            return await _context.Users.Select(x => x.ToUserDto()).ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user.ToUserDto();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(int id, PutUserDto putUserDto)
        {
            if (id != putUserDto.Id)
            {
                return BadRequest();
            }

            if (id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(id);
            }

            if (user.Username != putUserDto.Username && UsernameExists(putUserDto.Username))
            {
                ModelState.AddModelError(nameof(putUserDto.Username), "Username is already used.");
                return ValidationProblem(ModelState);
            }

            PasswordHasher<string> pw = new();
            user.UpdateProfile(putUserDto.Username, pw.HashPassword(putUserDto.Username, putUserDto.Password));
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> PostUser(PostUserDto postUserDto)
        {
            if (UsernameExists(postUserDto.Username))
            {
                ModelState.AddModelError(nameof(postUserDto.Username), "Username is already used.");
                return ValidationProblem(ModelState);
            }

            PasswordHasher<string> pw = new();
            postUserDto.Password = pw.HashPassword(postUserDto.Username, postUserDto.Password);

            var user = new User(postUserDto);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user.ToUserDto());
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
    }
}
