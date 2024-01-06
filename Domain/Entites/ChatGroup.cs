using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    public class ChatGroup: BaseEntities
    {
        [MaxLength(100)]
        public string GroupTitle { get; set; }
        [MaxLength(110)]
        public string GroupToken { get; set; }
        public long? ReciverId { get; set; }
        public bool IsPrivete { get; set; }
        public string ImageName { get; set; }
        public long OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User User { get; set; }
        [ForeignKey("ReciverId")]
        public virtual User Reciver { get; set; }
        public virtual ICollection<Chat> Chats { get; set;}
        public virtual ICollection<UserGroup> Groups { get; set; }
    }
}
