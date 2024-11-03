using OnlineShoppingApp.Business.Operations.Order.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineShoppingApp.WebApi.Models
{
    public class AddOrderRequest
    {
        public int UserId { get; set; }
        public List<OrderProductDto> Products { get; set; }
    }
}
