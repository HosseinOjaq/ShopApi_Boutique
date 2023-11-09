using Data.Repositories;
using Entities;
using Entities.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> AddOrderAsync(int userId, int productId, int count, CancellationToken cancellationToken);
        Task<Order> EditOrderBayid(int orderId, int orderDetailId, int count, CancellationToken cancellationToken);
        Task<List<UserOrderDto>> GetUserOrders(bool? IsFinaly, int userId, CancellationToken cancellationToken);
        Task<bool> Delete(int orderId, CancellationToken cancellationToken);

    }
}
