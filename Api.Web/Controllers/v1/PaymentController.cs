using Common;
using Data.Contracts;
using WebFramework.Api;
using System.Threading;
using WebFramework.Filters;
using Entities.DTOs.Payment;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace Api.Web.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiResultFilter]
    [EnableCors("AllowCors")]
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpPost("Book")]
        public async Task<IActionResult> FinalyOrder(int orderId, CancellationToken cancellationToken)
        {
            var result = await _paymentRepository.FinalyOrder(orderId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("FinalizeBook")]
        public async Task<ApiResult<PaymentVerificationResponseDto>> FinalyBuy(string authority, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var result = await _paymentRepository.FinalyBuy(authority, userId, cancellationToken);
            return Ok(result);
        }
    }
}