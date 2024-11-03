using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Data.Entities
{
    public class OrderEntity : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public ICollection<OrderProductEntity> OrderProducts { get; set; }
    }

    public class OrderConfiguration : BaseConfiguration<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            base.Configure(builder);
            builder.HasMany(o => o.OrderProducts)
                   .WithOne(op => op.Order)
                   .HasForeignKey(op => op.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
