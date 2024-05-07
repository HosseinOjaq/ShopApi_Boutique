using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Product : BaseEntity<int>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public string ShippingTime { get; set; }

        public int CategoryId { get; set; }

        [Range(0, 100, ErrorMessage = "discount is not valid. because must be between 0 to 100")]
        public int Discount { get; set; }

        public Category Category { get; set; }
        public ICollection<ShoppingItem> ShoppingItems { get; set; }
        public ICollection<ProductFile> ProductFiles { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<ProductRating> ProductRatings { get; set; }
        public ICollection<ProductTags> ProductTags { get; set; }
    }
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired();
            builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            builder.HasIndex(p => p.Title).IsUnique();
        }
    }
}