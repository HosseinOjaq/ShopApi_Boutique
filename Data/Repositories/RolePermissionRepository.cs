using Common;
using Data.Contracts;
using Entities.DTOs.Roles;
using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository, IScopedDependency
    {
        private readonly IPermissionGroupRepository _permissionGroup;
        public RolePermissionRepository(ApplicationDbContext dbContext, IPermissionGroupRepository permissionGroup) : base(dbContext)
        {
            _permissionGroup = permissionGroup;
        }
        public async Task<List<TreeViewItemModel>> GetPermissionsList(int? roleid = 0)
        {
            var treeItems = new List<TreeViewItemModel>();
            List<int> rolePermissions = new List<int>();

            if (roleid != 0)
            {
                rolePermissions = Table.Where(r => r.RoleID == roleid)
                    .Select(r => r.PermissionID).ToList();
            }

            var permissionGroups = _permissionGroup.Table.Include("Permissions")
                .OrderBy(pg => pg.PermissionGroupTitle).ToList();

            foreach (var permissionGroup in permissionGroups)
            {
                TreeViewItemModel item = new TreeViewItemModel()
                {
                    Id = permissionGroup.Id,
                    Text = permissionGroup.PermissionGroupTitleLocalized ?? permissionGroup.PermissionGroupTitle
                };
                var permissions = permissionGroup.Permissions.OrderBy(p => p.PermissionTitle).ToList();

                foreach (var permission in permissions)
                {
                    item.Items.Add(new TreeViewItemModel()
                    {
                        Id = permission.Id,
                        Text = permission.PermissionTitleLocalized ?? permission.PermissionTitle,
                        Checked = (roleid != null) && (rolePermissions.Contains(permission.Id))
                    });
                }
                treeItems.Add(item);
            }
            return treeItems;
        }
    }
}
