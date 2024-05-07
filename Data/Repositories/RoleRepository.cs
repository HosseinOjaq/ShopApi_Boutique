using Common;
using Data.Contracts;
using Entities;
using System.Collections.Generic;
using System.Linq;

namespace Data.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository, IScopedDependency
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public IEnumerable<Role> ReturnRoleService(int Id)
        {
            var exists = Table.Where(a => a.Id == Id).ToList();
            return exists;
        }
    }
}
