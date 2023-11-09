using Data.Repositories;
using Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IPermissionRepository:IRepository<Permission>
    {
    }
}
