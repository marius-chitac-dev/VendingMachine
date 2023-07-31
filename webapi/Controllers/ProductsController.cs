using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webapi.Core;
using webapi.Infrastructure;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly VendingMachineContext _context;

        public ProductsController(VendingMachineContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return await _context.Products.Select(p => p.ToProductDto()).ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product.ToProductDto();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutProduct(int id, PutProductDto putProductDto)
        {
            if (id != putProductDto.Id)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(id);
            }

            if (product.SellerId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Unauthorized();
            }

            product.Update(putProductDto.AmountAvailable, putProductDto.Cost, putProductDto.ProductName);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> PostProduct(PostProductDto postProductDto)
        {
            var product = new Product(postProductDto.AmountAvailable, postProductDto.Cost, postProductDto.ProductName, int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product.ToProductDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.SellerId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Unauthorized();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
