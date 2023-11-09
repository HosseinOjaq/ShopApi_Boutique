using Data.Repositories;
using Entities;
using Entities.DTOs.UserAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IUserAddressRepository : IRepository<UserAddress>
    {
        Task<UserAddress> GetAddressUser(int userId, CancellationToken cancellationToken);
        Task<UserAddress> EditAddressUser(UserAddressDto model, int userId, CancellationToken cancellationToken);
        Task<UserAddress> AddAddressUser(UserAddressDto model, int userId, CancellationToken cancellationToken);
    }
}
