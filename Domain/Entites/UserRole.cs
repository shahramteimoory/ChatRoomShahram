using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    public class UserRole:BaseEntities
    {
       public long RoleId { get; set; }
       public long UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
