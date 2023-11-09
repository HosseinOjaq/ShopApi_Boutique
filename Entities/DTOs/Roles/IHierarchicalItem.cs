using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Roles
{
    public interface IHierarchicalItem
    {
        int Id { get; }

        bool HasChildren { get; }

        bool Expanded { get; }
    }
}
