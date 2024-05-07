using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        public int Code { get; set; } = 1000;


        public virtual User User { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(p => p.User).WithMany(c => c.Orders).HasForeignKey(p => p.UserId);            
            builder.Property(o => o.Code)
                   .HasDefaultValueSql("NEXT VALUE FOR OrederCodeSequence");
        }
    }
}