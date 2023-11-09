using Common;
using Data.Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository, IScopedDependency
    {
        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
