using System.Linq;
using Data.Contracts;
using WebFramework.Api;
using System.Threading;
using Entities.DTOs.Roles;
using WebFramework.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;


namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [ApiController]
    [AllowAnonymous]
    [ApiResultFilter]
    [EnableCors("AllowCors")]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IRolePermissionRepository rolePermissionRepository;
        public PermissionController(IRolePermissionRepository rolePermissionRepository)
        {
            this.rolePermissionRepository = rolePermissionRepository;

        }

        [HttpGet(nameof(GetPermissionsList))]
        public async Task<ApiResult<List<TreeViewItemModel>>> GetPermissionsList(int? roleId = 0, CancellationToken cancellationToken = default)
        {
            var result = await rolePermissionRepository.GetPermissionsList(roleId, cancellationToken);

            if (result is null)
            {
                return NotFound("دسترسی یافت نشد");
            }
            return Ok(result);
        }

        [HttpGet(nameof(Create))]
        public async Task<ApiResult<List<TreeViewItemModel>>> Create(int roleId, int[] selectedPermissions, CancellationToken cancellationToken = default)
        {
            var result = rolePermissionRepository.Create(roleId, selectedPermissions);

            if (result is null)
            {
                return NotFound("دسترسی یافت نشد");
            }
            return Ok();
        }

        [HttpPost(nameof(DeletePermission))]
        public ApiResult DeletePermission(int roleId)
        {
            var rolePermissions = rolePermissionRepository.TableNoTracking.Where(rp => rp.RoleID == roleId).ToList();
            rolePermissionRepository.Entities.RemoveRange(rolePermissions);

            return Ok();
        }
    }
}
