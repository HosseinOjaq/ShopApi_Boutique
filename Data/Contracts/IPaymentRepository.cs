using Entities;
using System.Threading;
using Data.Repositories;
using System.Threading.Tasks;
using Entities.DTOs.Payment;

namespace Data.Contracts
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<string> FinalyOrder(int orderId, CancellationToken cancellationToken);
        Task<PaymentVerificationResponseDto> FinalyBuy(string authority, int userId, CancellationToken cancellationToken);
    }
}