using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Identity
{
    public class RolePermission : BaseEntity<int>
    {
        public RolePermission()
        {
        }
        public int RoleID { get; set; }
        [Column(Order = 2)]
        public int PermissionID { get; set; }

        // Navigation Properties

        [ForeignKey("RoleID")]
        public Role Role { get; set; }

        [ForeignKey("PermissionID")]
        public Permission Permission { get; set; }
    }
}
