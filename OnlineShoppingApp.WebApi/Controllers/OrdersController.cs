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
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
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
            var addOrderDto = new AddOrderDto
            {
                UserId = request.UserId,
                Products = request.Products,

            };

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
        [Authorize(Roles = "Admin")]
        [TimeControlFilter]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            var updateOrderDto = new UpdateOrderDto
            {
                Id = id,
                UserId = request.UserId,
                Products = request.Products
            };

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
