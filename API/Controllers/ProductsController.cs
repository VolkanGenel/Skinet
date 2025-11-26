using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    StoreContext _context;
    public ProductsController(StoreContext context)
    {
        _context =  context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
       List<Product> products = await _context.Products.ToListAsync();
       return products;
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product != null) return product;
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }
    // Aslında FromRoute ya da FromBody yazmaya gerek yok. Çünkü Dotnet primitive dataları otomatik olarak route'da
    //ya da query string de arar. Nesneleri ise body'de arar. Ancak belirtmek temiz koda daha uygun.
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
    {
        if (id != product.Id || !ProductExists(id)) return BadRequest("Could not update product");
        
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        return NotFound();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}