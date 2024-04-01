using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entites
{
    public class Chat:BaseEntities
    {
        public string ChatBody { get; set; }
        public string? FileAttach { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("GroupId")]
        public virtual ChatGroup ChatGroup { get; set; }
    }
}
