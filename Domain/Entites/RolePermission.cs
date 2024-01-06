using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    public class RolePermission: BaseEntities
    {
        public long RoleId { get; set; }
        public Permission Permission { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

    }
}
