using GlobalExceptionHandling.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ILogger<ProductController> _logger,IProductService _productService) : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            //try
            //{
                var product = _productService.GetById(id);
                return Ok(product);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Error fetching product {ProductId}", id);
            //    return StatusCode(500, "An error occurred");
            //}
        }
    }
}
