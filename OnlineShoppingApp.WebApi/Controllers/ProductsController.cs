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
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProducts();

            return Ok(products);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);

            if (product is null)
                return NotFound();
            else
                return Ok(product);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addProductDto = new AddProductDto
            {
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
            };

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



        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatchProduct(int id, decimal changeTo)
        {
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
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

