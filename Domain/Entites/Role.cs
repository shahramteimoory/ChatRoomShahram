using System.ComponentModel.DataAnnotations;

namespace Domain.Entites
{
    public class Role: BaseEntities
    {
        [MaxLength(50)]
        string Title { get; set; }

    }
}
