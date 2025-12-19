using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> repository /*, IProductRepository productRepository*/) : ControllerBase
{
    // ProductRepository _repository = repository; Class içinde constructor inject olduğu için artık gerek yok

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type,string? sort)
    {
        var spec = new ProductSpecification(brand, type, sort);
        var products = await repository.ListAsyncWithSpec(spec);
        return Ok(products);
    }

    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product != null) return product;
        return NotFound();
    }
    //
    // [HttpGet("byname/{name}")] // api/products/byname/telefon
    // public async Task<ActionResult<Product>> GetProduct(string name)
    // {
    //     var product = await repository.GetProductByNameAsync(name);
    //
    //     if (product != null) return product;
    //     return NotFound();
    // }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        repository.Add(product);
        if(await repository.SaveChangesAsync())
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        return BadRequest("Problem adding product");
    }
    // Aslında FromRoute ya da FromBody yazmaya gerek yok. Çünkü Dotnet primitive dataları otomatik olarak route'da
    //ya da query string de arar. Nesneleri ise body'de arar. Ancak belirtmek temiz koda daha uygun.
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
    {
        if (id != product.Id || !ProductExists(id)) return BadRequest("Could not update product");
        
            repository.Update(product);
            if (await repository.SaveChangesAsync())
            {
                return NoContent();
            }
            
        return BadRequest("Problem updating product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repository.GetByIdAsync(id);
        if (product != null)
        {
            repository.Remove(product);
            if (await repository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem deleting product");
        }
        return NotFound("Product not found");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        // TODO: Implement method
        // return Ok(await productRepository.GetBrandsAsync());
        var spec = new BrandListSpecification();
        return Ok(await repository.ListAsyncWithSpec(spec));
    }
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        // TODO: Implement method
        // return Ok(await productRepository.GetTypesAsync());
        var spec = new TypeListSpecification();
        return Ok(await repository.ListAsyncWithSpec(spec));
    }
    
    private bool ProductExists(int id)
    {
        return repository.Exists(id);
    }
}