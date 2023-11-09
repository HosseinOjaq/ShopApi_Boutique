using Data.Repositories;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IProductTagsRepository : IRepository<ProductTags>
    {
        Task AddProductTags(List<string> tag, int productId, CancellationToken cancellationToken);
    }

}
