using Data.Repositories;
using Entities.DTOs.Roles;
using Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IRolePermissionRepository:IRepository<RolePermission>
    {
        Task<List<TreeViewItemModel>> GetPermissionsList(int? roleid = 0);
    }
}
