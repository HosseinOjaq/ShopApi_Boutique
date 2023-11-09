using Common;
using Data.Contracts;
using Entities.Identity;

namespace Data.Repositories
{
    public class PermissionGroupRepository : Repository<PermissionGroup>, IPermissionGroupRepository, IScopedDependency
    {
        public PermissionGroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
