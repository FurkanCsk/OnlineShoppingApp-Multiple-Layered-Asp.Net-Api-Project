using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineShoppingApp.Data.Entities.UserEntity;

namespace OnlineShoppingApp.Business.Operations.Order.Dtos
{
    public class OrderUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Role UserType { get; set; }
    }
}
