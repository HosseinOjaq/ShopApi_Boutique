using Common;
using Data.Contracts;
using Entities.DTOs.Roles;
using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository, IScopedDependency
    {
        private readonly IPermissionGroupRepository _permissionGroup;
        private readonly IRoleRepository _roleRepository;

        public RolePermissionRepository(
            ApplicationDbContext dbContext,
            IPermissionGroupRepository permissionGroup,
            IRoleRepository roleRepository) : base(dbContext)
        {
            _permissionGroup = permissionGroup;
            _roleRepository = roleRepository;
        }

        public async Task Create(int rolId, int[] SelectedPermissions)
        {
            if (await _roleRepository.Table.AnyAsync(r => r.Id == rolId))
            {
                var existingPermissions = base.Table.Where(rp => rp.RoleID == rolId).ToList();
                if (SelectedPermissions != null)
                {
                    // Clear Existing RolePermissions:
                    Entities.RemoveRange(existingPermissions);

                    var groups = _permissionGroup.Table.Select(pg => pg.Id).ToList();
                    var addingItems = new List<RolePermission>();
                    foreach (var selectedPermission in SelectedPermissions)
                    {
                        if (!groups.Contains(selectedPermission))
                        {
                            addingItems.Add(new RolePermission
                            {
                                PermissionID = selectedPermission,
                                RoleID = rolId
                            });
                        }
                    }

                    if (addingItems.Any())
                    {
                        await Entities.AddRangeAsync(addingItems);
                    }
                }
            }
        }

        public async Task<List<TreeViewItemModel>> GetPermissionsList(int? roleid = 0, CancellationToken cancellationToken = default)
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

        public async Task<bool> HasPermission(int userId, string actionFullName)
        {
            return await (from UserRole in DbContext.UserRoles
                          join RolePermissions in DbContext.RolePermissions
                          on UserRole.RoleId equals RolePermissions.RoleID
                          join Permissions in DbContext.Permissions
                          on RolePermissions.PermissionID equals Permissions.Id
                          where UserRole.UserId == userId
                          && Permissions.ActionFullName == actionFullName
                          select RolePermissions.Id)
                .AnyAsync();
        }
    }
}