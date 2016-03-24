using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Commit : Parser.Models.Commit
    {
        [ForeignKey("Project")]
        public Guid ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
