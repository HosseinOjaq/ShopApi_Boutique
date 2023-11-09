using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class UserAddress : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public bool IsDefault { get; set; }

        public User  User { get; set; }
    }
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.HasOne(p => p.User).WithMany(c => c.UserAddresses).HasForeignKey(p => p.UserId);
        }
    }
}
