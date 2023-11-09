using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Entities.Identity
{
    public class PermissionGroup:BaseEntity<int>
    {
        public PermissionGroup()
        {
            Permissions = new List<Permission>();
        }

        [Display(Name = "عنوان گروه دسترسی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string PermissionGroupTitle { get; set; }

        public string PermissionGroupNamespace { get; set; }

        [Display(Name = "عنوان گروه دسترسی (بومی)")]
        public string PermissionGroupTitleLocalized { get; set; }

        public bool RequiresAuthorization { get; set; }

        // Navigation Properties
        public List<Permission> Permissions { get; set; }
    }
}
