using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;

namespace Undefined.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all products")]
        [SwaggerResponse(200, "List of products", typeof(IEnumerable<Product>))]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Attempt to search for all products in our database");
            var products = await _productRepository.GetAllProductsAsync();
            
            _logger.LogInformation("A user was able to use the search for all products");
            return Ok(products);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a product by ID")]
        [SwaggerResponse(200, "Product found", typeof(Product))]
        [SwaggerResponse(404, "Product not found")]
        public async Task<IActionResult> GetProductById(int id)
        {
            _logger.LogInformation("Attempt to search for a product in our database by ID");
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product is null)
            {
                _logger.LogCritical(
                    "There was an attempt to search for the product with id {Id}, but it was not found in our database", id);
                return NotFound();
            }

            _logger.LogInformation("A user was able to use the search for a product called {ProductName}", product.Name);
            return Ok(product);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add a new product")]
        [SwaggerResponse(201, "Product successfully added", typeof(Product))]
        [SwaggerResponse(400, "Invalid product data")]
        [SwaggerResponse(500, "Internal server error")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            _logger.LogInformation("Attempt to add a new product to our database");
            if (product is null)
            {
                _logger.LogCritical("Unsuccessful attempt to add a product. The provided product data was null or invalid");
                return BadRequest();
            }

            var addedProduct = await _productRepository.AddProductAsync(product);
            _logger.LogInformation("Product added successfully. ProductId: {Id}, Name: {ProductName}", product.Id, product.Name);
            return CreatedAtAction(nameof(GetProductById), new { id = addedProduct.Id }, addedProduct);
        }
    }
}
