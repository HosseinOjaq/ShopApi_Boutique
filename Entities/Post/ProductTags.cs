using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ProductTags:BaseEntity
    {
        public int TagId { get; set; }
        public int ProductId { get; set; }


        public Tags Tags { get; set; }
        public Product Product { get; set; }
    }
    public class ProductTagsConfiguration : IEntityTypeConfiguration<ProductTags>
    {
        public void Configure(EntityTypeBuilder<ProductTags> builder)
        {
            builder.HasOne(p => p.Tags).WithMany(c => c.productTags).HasForeignKey(p => p.TagId);
            builder.HasOne(p => p.Product).WithMany(c => c.productTags).HasForeignKey(p => p.ProductId);
        }
    }
}
