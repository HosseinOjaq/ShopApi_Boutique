using Data.Repositories;
using Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IProductLikesRepository : IRepository<ProductLikes>
    {
        Task<bool> CheckLike(int productId, int userId, CancellationToken cancellationToken);
        Task AddLike(int productId, int userId);
    }
}
