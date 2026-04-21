using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Controlers
{

    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
            [HttpGet]
            public IActionResult Get()
            {
            var products = new List<ProductsDomain>()
             {
            new() { Id = 1, Name="Product1", Price=100 },
            new() { Id = 2, Name="Product2", Price=156 },
            new() { Id = 3, Name="Product3", Price=120 },
              };
            return Ok(products);
        }
        
    }
}
