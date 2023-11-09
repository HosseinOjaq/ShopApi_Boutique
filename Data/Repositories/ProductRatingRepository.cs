using Common;
using Data.Contracts;
using Entities;
using Entities.DTOs.Order;
using Entities.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRatingRepository : Repository<ProductRating>, IProductRatingRepository, IScopedDependency
    {
       

        public ProductRatingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
          
        }

        public async Task<int> AddProductRating(int productId, int userId, int rate, CancellationToken cancellationToken)
        {
            var productRating = await base.Table.Where(a => a.ProductId == productId && a.UserId == userId).SingleOrDefaultAsync(cancellationToken);
            productRating = new ProductRating()
            {
                Rating = rate,
                UserId = userId,
                Id = productId
            };
            await base.AddAsync(productRating, cancellationToken);
            return productRating.Rating;
        }
        public async Task<double> CalculateRating(int productId, CancellationToken cancellationToken)
        {
            var productRatings = await base.Table.Where(a => a.ProductId == productId).ToListAsync(cancellationToken);
            return productRatings.Average(x => x.Rating);
        }

    }
}