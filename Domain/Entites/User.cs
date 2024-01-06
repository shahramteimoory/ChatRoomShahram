using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    public class User: BaseEntities
    {
        [MaxLength(100)]
        public string UserName { get; set; }
        [MaxLength(100)]
        [MinLength(6)]
        public string Password { get; set; }
        [MaxLength(110)]
        public string Avatar { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ChatGroup> ChatGroups { get; set; }
        [InverseProperty("Reciver")]
        public virtual ICollection<ChatGroup> PrivateGroups { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}
