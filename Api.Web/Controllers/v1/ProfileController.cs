using Common;
using Data.Contracts;
using Data.Repositories;
using Entities;
using Entities.DTOs.Order;
using Entities.DTOs.UserAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IUserAddressRepository userAddressRepository;
        private readonly IOrderRepository orderRepository;

        public ProfileController(IUserRepository userRepository, IUserAddressRepository userAddressRepository, IOrderRepository orderRepository)
        {
            this.userRepository = userRepository;
            this.userAddressRepository = userAddressRepository;
            this.orderRepository = orderRepository;
        }
        [HttpPost(nameof(AddAddressUser))]
        public async Task<ApiResult<UserAddress>> AddAddressUser(UserAddressDto model, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var useraddress = await userAddressRepository.AddAddressUser(model, 2, cancellationToken);
            return useraddress;
        }
        [HttpGet(nameof(GetUserOrders))]
        public async Task<ApiResult<List<UserOrderDto>>> GetUserOrders(bool IsFinaly, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var result = await orderRepository.GetUserOrders(IsFinaly, 2, cancellationToken);
            return Ok(result);
        }
        [HttpPut(nameof(EditAddressUser))]
        public async Task<ApiResult<UserAddress>> EditAddressUser(UserAddressDto model, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var userAddress = await userAddressRepository.EditAddressUser(model, 2, cancellationToken);
            return userAddress;
        }
        [HttpGet(nameof(GetUserAddress))]
        public async Task<ApiResult<UserAddress>> GetUserAddress(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var result = await userAddressRepository.GetAddressUser(2, cancellationToken);
            return result;
        }
        [HttpGet(nameof(GetUserProfile))]
        public async Task<ApiResult<User>> GetUserProfile(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var result = await userRepository.GetByIdAsync( cancellationToken, 2);
            return result;
        }
    }
}
