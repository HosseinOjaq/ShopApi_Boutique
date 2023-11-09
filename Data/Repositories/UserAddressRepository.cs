using Common;
using Data.Contracts;
using Entities;
using Entities.DTOs.UserAddress;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserAddressRepository : Repository<UserAddress>, IUserAddressRepository, IScopedDependency
    {
        public UserAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<UserAddress> AddAddressUser(UserAddressDto model, int userId, CancellationToken cancellationToken)
        {
            var useraddress = new UserAddress
            {
                Title = model.Title,
                UserId = userId,
                Address = model.Address,
                IsDefault = model.IsDefault,
                Postalcode = model.Postalcode,
            };
            await AddAsync(useraddress, cancellationToken);
            return useraddress;
        }
        public async Task<UserAddress> EditAddressUser(UserAddressDto model, int userId, CancellationToken cancellationToken)
        {
            var user = await Table.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            user.Address = model.Address;
            user.IsDefault = model.IsDefault;
            user.Postalcode = model.Postalcode;
            user.Title = model.Title;
            user.UserId = userId;
            await UpdateAsync(user, cancellationToken);
            return user;
        }
        public async Task<UserAddress> GetAddressUser(int userId, CancellationToken cancellationToken)
        {
            var result = await Table.SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
            return result;
        }
    }
}
