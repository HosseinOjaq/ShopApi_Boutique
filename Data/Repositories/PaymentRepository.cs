using Common;
using Data.Contracts;
using Entities;


namespace Data.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository, IScopedDependency
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
