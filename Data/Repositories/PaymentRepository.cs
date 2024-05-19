using Common;
using System;
using Entities;
using System.Linq;
using Data.Contracts;
using Entities.Enums;
using System.Threading;
using Entities.DTOs.Payment;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Data.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository, IScopedDependency
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        public PaymentRepository(ApplicationDbContext dbContext, IOrderRepository orderRepository, IUserRepository userRepository) : base(dbContext)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<string> FinalyOrder(int orderId, CancellationToken cancellationToken)
        {
            var existsPayment = await Table
                    .SingleOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
            if (existsPayment is not null && existsPayment.Status == PaymentStatus.Finally)
                return "این پیش فاکتور نهایی شده است.";

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
                await UpdateAsync(existsPayment, cancellationToken);
                return req.Link;
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
            await AddAsync(payment, cancellationToken);
            return req.Link;
        }

        public async Task<PaymentVerificationResponseDto> FinalyBuy(string authority, int userId, CancellationToken cancellationToken)
        {
            var result = new PaymentVerificationResponseDto();
            var payment = await Table
                                .Include(x => x.Order)
                                .SingleOrDefaultAsync(x => x.Authority == authority &&
                                                      x.Status == PaymentStatus.Pending &&
                                                      x.Order.UserId == userId, cancellationToken);
            if (payment is null)
                result.Message = "درخواست نامعتبر می باشد.";

            var zarinPal = new ZarinpalSandbox.Payment((int)payment.Price);
            var verivication = await zarinPal.Verification(authority);
            if (verivication is null)
                result.Message = "درخواست نامعتبر می باشد.";

            payment.StatusCode = verivication.Status;
            if (payment.StatusCode == 100)
            {
                payment.RefrencID = verivication.RefId;
                payment.Status = PaymentStatus.Finally;

                var sumPayments = await TableNoTracking
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
            await UpdateAsync(payment, cancellationToken);

            if (verivication.Status == 101)
                result.Message = "درخواست پرداخت تکراری";

            if (verivication.Status == -21)
                result.Message = "کنسل شده توسط کاربر";

            if (verivication.Status != 100)
                result.Message = "پرداخت با شکست روبه رو شد";

            result.Status = verivication.Status;
            result.Message = "باموفقیت خرید انجام شد";
            result.RefId = verivication.RefId;
            return result;
        }

    }
}
