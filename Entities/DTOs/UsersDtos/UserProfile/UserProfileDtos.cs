using Entities;

namespace MyApi.Models.UserProfile
{
    public class CreateUserDto
    {
        public string FullName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
    }
}
