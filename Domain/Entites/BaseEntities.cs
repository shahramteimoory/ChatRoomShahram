using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entites
{
    public class BaseEntities
    {
        [Key]
        public long Id { get; set; }
        public DateTime createDate { get; set; } = DateTime.UtcNow;
    }
}
