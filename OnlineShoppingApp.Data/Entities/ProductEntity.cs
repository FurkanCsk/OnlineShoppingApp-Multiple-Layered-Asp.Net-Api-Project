using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Data.Entities
{
    public class ProductEntity : BaseEntity
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public ICollection<OrderProductEntity> OrderProducts { get; set; }
    }

    public class ProductConfiguration : BaseConfiguration<ProductEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            base.Configure(builder);
            builder.HasMany(p => p.OrderProducts)
                   .WithOne(op => op.Product)
                   .HasForeignKey(op => op.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
