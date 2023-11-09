using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ProductFile : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public string FileName { get; set; }
        public bool MainPicture { get; set; }


        public Product Product { get; set; }
    }
    public class ProductFileConfiguration : IEntityTypeConfiguration<ProductFile>
    {
        public void Configure(EntityTypeBuilder<ProductFile> builder)
        {
            builder.HasOne(p => p.Product).WithMany(c => c.ProductFiles).HasForeignKey(p => p.ProductId);
        }
    }
}
