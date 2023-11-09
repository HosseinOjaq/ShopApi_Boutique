using Data.Repositories;
using Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IProductRatingRepository : IRepository<ProductRating>
    {
        Task<double> CalculateRating(int productId, CancellationToken cancellationToken);
    }
}
