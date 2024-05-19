using Entities;
using Data.Contracts;
using WebFramework.Api;
using Entities.Attributes;
using WebFramework.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    [IgnorePermissionCheck]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        [HttpPost(nameof(Add))]
        public async Task<ApiResult> Add(Role role)
        {
            roleRepository.Entities.Add(role);
            return Ok();
        }
        [HttpPost(nameof(Delete))]
        public async Task<ApiResult> Delete(int roleId)
        {
            var role = await roleRepository.Table.SingleOrDefaultAsync(x => x.Id == roleId);
            var result = roleRepository.Entities.Remove(role);
            return Ok();
        }
    }
}
