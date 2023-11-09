using Data.Contracts;
using Entities.DTOs.Roles;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebFramework.Api;
using System.Linq;
using Entities.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using WebFramework.Filters;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [Authorize]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolesController(IRolePermissionRepository rolePermissionRepository, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpGet(nameof(GetAllPermision))]
        public async Task<ApiResult<List<TreeViewItemModel>>> GetAllPermision(int? role)
        {
            var permission = await _rolePermissionRepository.GetPermissionsList(role);
            return permission;
        }
        [HttpGet(nameof(GetAllRoles))]
        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        public async Task<ApiResult<List<RoleViewModel>>> GetAllRoles()
        {
            var roles = (_roleManager.Roles.ToList().Select(r => new RoleViewModel()
            {
                RoleName = r.Name,
                RoleNameLocalized = r.RoleNameLocalized,
                RoleID = r.Id
            })).OrderBy(r => r.RoleName).ToList();

            return roles;
        }
    }
}
