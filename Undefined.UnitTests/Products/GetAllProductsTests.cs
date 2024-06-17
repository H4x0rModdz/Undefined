using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Products
{
    public class GetAllProductsTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public GetAllProductsTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ValidRequest_ReturnsOk()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };
            _mockProductRepository.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(products);

            var result = await _controller.GetAllProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count);
        }
    }
}
