using Common;
using Data.Contracts;
using Entities.Identity;

namespace Data.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository, IScopedDependency
    {
        public PermissionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
