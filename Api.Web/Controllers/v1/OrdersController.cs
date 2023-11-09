using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class OrdersController : Controller
    {
        private readonly IOrderRepository orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        [HttpPost(nameof(Add))]
        public async Task<ApiResult<Order>> Add(int productId, int count, CancellationToken cancellationToken)
        {
            //var userid = HttpContext.User.Identity.GetUserId<int>();
            var result = await orderRepository.AddOrderAsync(2, productId, count, cancellationToken);
            if (result is null)
                return BadRequest();
            return Ok(result);
        }

        [HttpPost(nameof(Edit))]
        public async Task<ApiResult<Order>> Edit(int orderId, int orderDetailId, int count, CancellationToken cancellationToken)
        {
            var result = await orderRepository.EditOrderBayid(orderId, orderDetailId, count, cancellationToken);
            if (result is null)
                return BadRequest();

            return Ok(result);
        }
    }
}
