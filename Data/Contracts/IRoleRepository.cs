﻿using Data.Repositories;
using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IRoleRepository : IRepository<Role>
    {
        IEnumerable<Role> ReturnRoleService(int Id);
    }
}
