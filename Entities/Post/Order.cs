using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Order : BaseEntity
    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal OrderSum { get; set; }

        public bool IsFinaly { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }


        public virtual User User { get; set; }        
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(p => p.User).WithMany(c => c.Orders).HasForeignKey(p => p.UserId);
        }
    }
}
