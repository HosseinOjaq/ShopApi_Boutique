using Common;
using Data.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductLikesRpositories : Repository<ProductLikes>, IProductLikesRepository, IScopedDependency
    {
        public ProductLikesRpositories(ApplicationDbContext dbContext) : base(dbContext) { }


        public async Task<bool> CheckLike(int productId, int userId, CancellationToken cancellationToken)
        {
            var liked = await Table.SingleOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId, cancellationToken: cancellationToken);
            return liked is not null;
        }
        public async Task AddLike(int productId, int userId)
        {
            var liked = await Table.SingleOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId);
            if (liked == null)
            {
                var like = new ProductLikes
                {
                    ProductId = productId,
                    UserId = userId,
                };
                var Newlike = await Entities.AddAsync(like);
            }
            else
            {
                Entities.Remove(liked);
            }
        }
    }
}
