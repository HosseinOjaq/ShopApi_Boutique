using System;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class Payment : BaseEntity
    {
        public string Authority { get; set; }
        [Required]
        public int OrderId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int StatusCode { get; set; }

        public PaymentStatus Status { get; set; }

        [Required]
        public long RefrencID { get; set; }

        [Required]
        public decimal Price { get; set; }

         
        public Order Order { get; set; }
    }
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(p => p.Order).WithMany(c => c.Payments).HasForeignKey(p => p.OrderId);
        }
    }
}
