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
        public TechnologyType Type { get; set; }

        public bool UpdateFromGit { get; set; }

        public bool Verified { get; set; }

        public long Count { get; set; }

        [MaxLength(32)]
        public string Unit { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public DateTime Begin { get; set; }

        [NotMapped]
        public long TotalDays
        {
            get
            {
                return (long)(DateTime.Now - Begin).TotalDays;
            }
        }

        [NotMapped]
        public string Display
        {
            get
            {
                if (TotalDays < 1)
                    return "1天";
                if (TotalDays < 30)
                    return TotalDays + "天";
                if (TotalDays < 365)
                    return (TotalDays / 30).ToString() + "个月";
                return (TotalDays / 365).ToString() + "年";
            }
        }

        public virtual User User { get; set; }
    }
}
