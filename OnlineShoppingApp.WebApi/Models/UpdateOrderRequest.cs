
using OnlineShoppingApp.Business.Operations.Order.Dtos;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingApp.WebApi.Models
{
    public class UpdateOrderRequest
    {
        [Required]
        public int UserId { get; set; }
        public List<OrderProductDto> Products { get; set; }
    }
}
