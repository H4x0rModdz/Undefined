using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Products
{
    public class AddProductTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public AddProductTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ValidProduct_ReturnsCreatedAtAction()
        {
            var product = new Product { Id = 1, Name = "Product 1" };
            _mockProductRepository.Setup(repo => repo.AddProductAsync(product)).ReturnsAsync(product);

            var result = await _controller.AddProduct(product);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(product.Id, returnedProduct.Id);
        }

        [Fact]
        public async Task InvalidProduct_ReturnsBadRequest()
        {
            var result = await _controller.AddProduct(null);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
