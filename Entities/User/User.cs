using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{
    public class User : IdentityUser<int>, IEntity<int>
    {
        public User()
        {
            IsActive = true;
        }
        [StringLength(100)]
        public string FullName { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }


        public ICollection<ShoppingItem> ShippingItems { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ProductRating> ProductRatings { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(p => p.UserName).IsRequired();
        }
    }

    public enum GenderType
    {
        [Display(Name = "مرد")]
        Male = 1,

        [Display(Name = "زن")]
        Female = 2
    }
}
