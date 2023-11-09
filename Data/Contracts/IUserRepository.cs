using System.Threading;
using System.Threading.Tasks;
using Entities;

namespace Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {        
        Task AddAsync(User user, string password, CancellationToken cancellationToken);        
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);
    }
}