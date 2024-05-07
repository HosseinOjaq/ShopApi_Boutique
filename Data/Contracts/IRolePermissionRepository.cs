using Data.Repositories;
using Entities.Identity;
using Entities.DTOs.Roles;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data.Contracts
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<List<TreeViewItemModel>> GetPermissionsList(int? roleid = 0);
        Task Create(int rolId, int[] SelectedPermissions);
        Task<bool> HasPermission(int userId, string actionFullName);
    }
}
