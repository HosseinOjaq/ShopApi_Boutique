using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class ProductRating : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Rating { get; set; }


        public User User { get; set; }
        public Product Product { get; set; }
    }
    public class RatingConfiguration : IEntityTypeConfiguration<ProductRating>
    {
        public void Configure(EntityTypeBuilder<ProductRating> builder)
        {
            builder.HasOne(p => p.Product).WithMany(c => c.ProductRatings).HasForeignKey(p => p.ProductId);
            builder.HasOne(p => p.User).WithMany(c => c.ProductRatings).HasForeignKey(p => p.UserId);
        }
    }
}
