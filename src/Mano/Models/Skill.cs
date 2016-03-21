using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Skill
    {
        public Guid Id { get; set; }

        [MaxLength(64)]
        public string Title { get; set; }

        [MaxLength(64)]
        public string Type { get; set; }

        public bool UpdateFromGit { get; set; }

        public bool Verified { get; set; }

        public long Count { get; set; }

        [MaxLength(32)]
        public string Unit { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual User User { get; set; }
    }
}
