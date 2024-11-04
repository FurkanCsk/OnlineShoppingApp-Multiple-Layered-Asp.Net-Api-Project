using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingApp.Business.Operations.Order;
using OnlineShoppingApp.Business.Operations.Order.Dtos;
using OnlineShoppingApp.WebApi.Filters;
using OnlineShoppingApp.WebApi.Models;

namespace OnlineShoppingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService; // Service for order-related operations

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService; // Injecting the order service through dependency injection
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            // Retrieve the list of orders
            var orders = await _orderService.GetOrders();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            // Retrieve a specific order by ID
            var order = await _orderService.GetOrder(id);


            if (order is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(order);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderRequest  request)
        {
            // Create a DTO for adding an order
            var addOrderDto = new AddOrderDto
            {
                UserId = request.UserId,
                Products = request.Products,

            };

            // Attempt to add the order using the order service
            var result = await _orderService.AddOrder(addOrderDto);

            if(!result.IsSucceed)
            {
                return BadRequest(result.Message);
            }
            else
            {
                return Ok("Order added successfully.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Attempt to delete the order using the order service
            var result = await _orderService.DeleteOrder(id);
            if(!result.IsSucceed)
            {
                return NotFound(result.Message);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only allow Admin role to update orders
        [TimeControlFilter] // Custom filter to control access time
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            // Create a DTO for updating an order
            var updateOrderDto = new UpdateOrderDto
            {
                Id = id,
                UserId = request.UserId,
                Products = request.Products
            };

            // Attempt to update the order using the order service
            var result = await _orderService.UpdateOrder(updateOrderDto);

            if(!result.IsSucceed)
            {
                return NotFound(result.Message);
            }
            else
            {
                return await GetOrder(id);
            }
        }


    }
}
