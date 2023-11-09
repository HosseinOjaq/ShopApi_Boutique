using Common;
using Data.Contracts;
using Entities;

namespace Data.Repositories
{
    public class ProductFileRepository : Repository<ProductFile>, IProductFileRepository, IScopedDependency
    {
        public ProductFileRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
