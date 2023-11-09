using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class OrderDetail : BaseEntity
    {

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasOne(p => p.Product).WithMany(c => c.OrderDetails).HasForeignKey(p => p.ProductId);
            builder.HasOne(p => p.Order).WithMany(c => c.OrderDetails).HasForeignKey(p => p.OrderId);
        }
    }
}
