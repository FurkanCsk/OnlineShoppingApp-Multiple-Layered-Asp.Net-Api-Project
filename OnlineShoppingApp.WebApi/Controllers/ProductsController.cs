using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingApp.Business.Operations.Product;
using OnlineShoppingApp.Business.Operations.Product.Dtos;
using OnlineShoppingApp.WebApi.Models;

namespace OnlineShoppingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService; // Service for product-related operations

        public ProductsController(IProductService productService)
        {
            _productService = productService; // Injecting the product service through dependency injection
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // Retrieve the list of products
            var products = await _productService.GetProducts();

            return Ok(products);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            // Retrieve a specific product by ID
            var product = await _productService.GetProduct(id);

            if (product is null)
                return NotFound();
            else
                return Ok(product);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")] // Only allow Admin role to add products
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a DTO for adding a product
            var addProductDto = new AddProductDto
            {
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
            };

            // Attempt to add the product using the product service
            var result = await _productService.AddProduct(addProductDto);

            if (result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Message);
            }
        }


        [HttpPatch("{id}/price")] // Route for adjusting the price of a product
        [Authorize(Roles = "Admin")] // Only allow Admin role to adjust prices
        public async Task<IActionResult> PatchProduct(int id, decimal changeTo)
        {
            // Attempt to adjust the product price using the product service
            var result = await _productService.AdjustProductPrice(id, changeTo);

            if (result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        [HttpDelete("{id}")] // Route for deleting a product
        [Authorize(Roles = "Admin")] // Only allow Admin role to delete products
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Attempt to delete the product using the product service
            var result = await _productService.DeleteProduct(id);

            if (result.IsSucceed)
            {
                return Ok();
            }
            else
            {
                return NotFound(result.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            // Create a DTO for updating a product
            var updateProductDto = new UpdateProductDto
            {
                Id = id,
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            var result = await _productService.UpdateProduct(updateProductDto);

            if (result.IsSucceed)
            {
                return await GetProduct(id);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
    }
}

