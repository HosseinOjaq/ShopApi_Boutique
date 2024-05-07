using Sentry;
using Common;
using System;
using Entities;
using System.Linq;
using Entities.Enums;
using Data.Contracts;
using WebFramework.Api;
using System.Threading;
using Data.Repositories;
using WebFramework.Filters;
using ZarinpalSandbox.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public PaymentController(IPaymentRepository paymentRepository, IUserRepository userRepository,
                                      IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [HttpPost("Book")]
        public async Task<IActionResult> FinalyOrder(int orderId, CancellationToken cancellationToken)
        {
            var existsPayment = await _paymentRepository.Table
                                .SingleOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
            if (existsPayment is not null && existsPayment.Status == PaymentStatus.Finally)
                return Ok("این پیش فاکتور نهایی شده است.");

            var order = await _orderRepository.Table.SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);
            var User = _userRepository.Table.Where(x => x.Id == order.UserId).SingleOrDefault();
            var zarinPal = new ZarinpalSandbox.Payment((int)order.OrderSum);
            var callbackUrl = "https://localhost:44339/api/Payment/FinalizeBook";
            var desciription = $"خرید بابت پیش فاکتور {order.Code}";
            var Email = User.Email;
            var mobile = User.PhoneNumber;
            var req = await zarinPal.PaymentRequest(desciription, callbackUrl, Email, mobile);
            if (existsPayment is not null && existsPayment.Status != PaymentStatus.Finally)
            {
                existsPayment.Authority = req.Authority;
                existsPayment.Price = order.OrderSum;
                await _paymentRepository.UpdateAsync(existsPayment, cancellationToken);
                return Ok(req.Link);
            }
            var payment = new Payment
            {
                Authority = req.Authority,
                Date = DateTime.Now,
                OrderId = orderId,
                Price = order.OrderSum,
                StatusCode = 0,
                Status = PaymentStatus.Pending,
                RefrencID = 0,
            };
            await _paymentRepository.AddAsync(payment, cancellationToken);
            return Ok(req.Link);
        }

        [HttpGet("FinalizeBook")]
        public async Task<ApiResult<PaymentVerificationResponse>> FinalyBuy(string authority, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var payment = await _paymentRepository.Table
                                .Include(x => x.Order)
                                .SingleOrDefaultAsync(x => x.Authority == authority &&
                                                      x.Status == PaymentStatus.Pending &&
                                                      x.Order.UserId == userId, cancellationToken);

            if (payment is null)
                return BadRequest("درخواست نامعتبر می باشد.");

            var zarinPal = new ZarinpalSandbox.Payment((int)payment.Price);
            var verivication = await zarinPal.Verification(authority);
            if (verivication is null)
                return BadRequest("درخواست نامعتبر.");

            payment.StatusCode = verivication.Status;
            if (payment.StatusCode == 100)
            {
                payment.RefrencID = verivication.RefId;
                payment.Status = PaymentStatus.Finally;

                var sumPayments = await _paymentRepository.TableNoTracking
                                        .Where(x => x.OrderId == payment.OrderId && x.Status == PaymentStatus.Finally)
                                        .SumAsync(x => x.Price);               
                var order = await _orderRepository.Table.SingleAsync(x => x.Id == payment.OrderId, cancellationToken);
                if (sumPayments == 0)
                    sumPayments = order.OrderSum;
                if (sumPayments >= order.OrderSum)
                {
                    order.IsFinaly = true;
                    await _orderRepository.UpdateAsync(order, cancellationToken);
                }
            }
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            if (verivication.Status == 101)
                return BadRequest("درخواست پرداخت تکراری");

            if (verivication.Status == -21)
                return BadRequest("کنسل شده توسط کاربر");

            if (verivication.Status != 100)
                return BadRequest("پرداخت با شکست روبه رو شد");

            return Ok(verivication);
        }
    }
}