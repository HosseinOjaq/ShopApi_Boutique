using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.UserAddress
{
    public class UserAddressDto
    {
        public string Title { get; set; }
        public string Address { get; set; }
        public string Postalcode { get; set; }
        public bool IsDefault { get; set; }
    }
}
