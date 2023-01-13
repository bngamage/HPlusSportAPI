using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HPlusSport.API.Controllers
{
    // routing: which url maps to which end-point.
    // basically the whole controller has a one url and the type of the http method and the route-template will decide which actionMethod to invoke.
    [Route("api/[controller]")]
    [ApiController] // this makes many features work out of the box (ex: serilaizing data, model validation and detailed explanation of exceptions etc.)
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        // DBContext is injected in constructor
        public ProductsController(ShopContext context) { 
            _context = context;

            // this will trigger OnModelCreating() in ShopContext and do sample data seeding
            _context.Database.EnsureCreated();
        }

        [HttpGet] // returns an ActionResult (which has status codes for different results) with the payload
        public async Task<ActionResult> GetAllProducts() {
            var products = await _context.Products.ToArrayAsync();

            // http OK response with payload of Products
            return Ok(products);
        }

        [HttpGet("{id}")] // route template (id attribute) will be properly passed to the actionMethod via id parameter by binding
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task <ActionResult<Product>> PostProduct([FromBody] Product product) // we do model-binding by using "FromBody"
        {
            // model validation : we dont use it as APIController provides it out of the box
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // in REST when add somthing, we should  provide the uri/ object which is representation of the newly added item as the response
            return CreatedAtAction(
                "GetProduct", // ActionName
                new { id = product.Id }, // productId
                product);
        }

        [HttpPut("{id}")] // using of route template for "Id"
        public async Task<ActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (product.Id != id) { 
                return BadRequest(); 
            }

            _context.Entry(product).State = EntityState.Modified;

            try { 

                await _context.SaveChangesAsync();
            } 
            // the product is being changed or it is deleted
            catch(DbUpdateConcurrencyException ex) { 
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                } else
                {
                    // we throw the error, it will be serialized into json
                    throw ex;
                }
            
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);

        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteProducts([FromQuery]int[] ids)
        {var products = new List<Product>();
            // if one of the ids are not met then etire delete operation is aborted
            foreach (var id in ids) 
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);

        }
    }
}
