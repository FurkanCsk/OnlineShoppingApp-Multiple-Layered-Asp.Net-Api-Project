using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Business.Operations.Order.Dtos
{
    public class AddOrderDto
    {
        public int UserId { get; set; }
        public List<OrderProductDto> Products { get; set; }
    }
}
