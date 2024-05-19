using Data.Repositories;
using Entities.Identity;
using Entities.DTOs.Roles;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Data.Contracts
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<List<TreeViewItemModel>> GetPermissionsList(int? roleid = 0 , CancellationToken cancellationToken = default);
        Task Create(int rolId, int[] SelectedPermissions);
        Task<bool> HasPermission(int userId, string actionFullName);
    }
}
