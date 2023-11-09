using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entities
{
    public class ShoppingItem : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime Created_At { get; set; }


        public Product Product { get; set; }
        public User User { get; set; }
    }
    public class ShippingItemConfiguration : IEntityTypeConfiguration<ShoppingItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingItem> builder)
        {
            builder.HasOne(p => p.User).WithMany(c => c.ShippingItems).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.Product).WithMany(c => c.ShoppingItems).HasForeignKey(p => p.ProductId);
        }
    }
}
