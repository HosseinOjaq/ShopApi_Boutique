using System;
using Entities;
using Services;
using WebFramework.Api;
using System.Threading;
using Common.Exceptions;
using Data.Repositories;
using Entities.DTOs.Token;
using WebFramework.Filters;
using Entities.DTOs.UserDtos;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MyApi.Controllers.v1
{
    [AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UsersController> logger;
        private readonly IJwtService jwtService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IJwtService jwtService,
            UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.jwtService = jwtService;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        [HttpPost(nameof(SignUp))]
        public virtual async Task<ApiResult<User>> SignUp(UserDto userDto, CancellationToken cancellationToken)
        {
            var isExists = await userManager.Users.AnyAsync(x => x.UserName == userDto.UserName, cancellationToken: cancellationToken);
            if (isExists)
                return BadRequest("نام کاربری تکراری است");
            var user = new User
            {
                UserName = userDto.Email,
                Email = userDto.Email
            };
            var result = await userManager.CreateAsync(user, userDto.Password);
            return user;
        }

        /// <summary>
        /// This method generate JWT Token
        /// </summary>
        /// <param name="tokenRequest">The information of token request</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(nameof(Token))]
        [EnableCors("AllowCors")]
        public virtual async Task<ActionResult> Token([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            if (!tokenRequest.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                throw new Exception("OAuth flow is not password.");

            var user = await userManager.FindByNameAsync(tokenRequest.username);
            if (user is null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, tokenRequest.password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var jwt = await jwtService.GenerateAsync(user);
            return new JsonResult(jwt);
        }
    }
}