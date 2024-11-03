using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingApp.Data.Entities
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public Role UserType { get; set; }

        public ICollection<OrderEntity> Orders { get; set; }

        public enum Role
        {
            User,Admin
        }

        public class UserConfiguration : BaseConfiguration<UserEntity>
        {
            public override void Configure(EntityTypeBuilder<UserEntity> builder)
            {
                builder.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(40);

                builder.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(40);

                builder.Property(x => x.Email)
                    .IsRequired();

                builder.HasIndex(x => x.Email)
                    .IsUnique();


                base.Configure(builder);
            }
        }

    }
}
