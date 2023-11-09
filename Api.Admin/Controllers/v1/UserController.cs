using Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using WebFramework.Api;
using WebFramework.Filters;
using Microsoft.EntityFrameworkCore;
using Entities.DTOs.UserDtos;
using MyApi.Models.UserProfile;
using Common.Utilities;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserController> logger;
        private readonly IJwtService jwtService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IJwtService jwtService,
            UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.jwtService = jwtService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        [HttpGet]        
        public async Task<ActionResult<List<User>>> GetAll(CancellationToken cancellationToken)
        {
            #region TokenValue
            //var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);
            //var securityStamp = HttpContext.User.Identity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
            #endregion

            var users = await userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return users;
        }

        [HttpGet(nameof(GetBayId))]
        public async Task<ApiResult<User>> GetBayId(CancellationToken cancellationToken)
        {
            var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            var user = await userManager.FindByIdAsync(userIdInt.ToString());
            if (user is null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPut]
        public virtual async Task<ApiResult> Update(int id, CreateUserDto user, CancellationToken cancellationToken)
        {
            var updateUser = await userManager.FindByIdAsync(id.ToString());
            updateUser.FullName = user.FullName;
            updateUser.Age = user.Age;
            updateUser.NormalizedUserName = user.NormalizedUserName;
            updateUser.Gender = user.Gender;
            updateUser.PhoneNumber = user.PhoneNumber;
            await userManager.UpdateAsync(updateUser);            
            return Ok();
        }

        [HttpDelete]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            await userManager.DeleteAsync(user);
            return Ok();
        }

        [HttpPost(nameof(Logout))]
        public async Task<ApiResult> Logout(CancellationToken cancellationToken)
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
    }
}