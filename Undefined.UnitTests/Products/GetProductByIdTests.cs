using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Undefined.Domain.Models;
using Undefined.Domain.Repositories;
using Undefined.WebApi.Controllers;

namespace Undefined.UnitTests.Products
{
    public class GetProductByIdTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public GetProductByIdTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockProductRepository.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ValidProduct_ReturnsOk()
        {
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product 1" };
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.GetProductById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        [Fact]
        public async Task InvalidProduct_ReturnsNotFound()
        {
            var productId = 1;
            _mockProductRepository.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            var result = await _controller.GetProductById(productId);
 
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
