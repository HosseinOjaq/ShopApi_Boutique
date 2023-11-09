using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Entities.DTOs.Roles
{
    public class RoleViewModel
    {
        [Key]
        public int RoleID { get; set; }
        [Display(Name = "نام گروه کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string RoleName { get; set; }

        [Display(Name = "نام گروه کاربری (بومی)")]
        public string RoleNameLocalized { get; set; }
    }
}
