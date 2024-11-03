using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingApp.WebApi.Models
{
    public class UpdateProductRequest
    {
        [Required]
        [Length(3, 50)]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
    }
}
