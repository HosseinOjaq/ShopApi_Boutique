using Common;
using Data.Contracts;
using Entities;

namespace Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository, IScopedDependency
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
