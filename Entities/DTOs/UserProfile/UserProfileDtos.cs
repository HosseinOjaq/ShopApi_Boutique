using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.UserProfile
{
    public class UsersDto
    {
        public string FullName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }

    }
}
