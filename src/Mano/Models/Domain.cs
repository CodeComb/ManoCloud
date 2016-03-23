using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Domain
    {
        public Guid Id { get; set; }

        [MaxLength(64)]
        public string DomainName { get; set; }
        
        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }

        public bool Default { get; set; }

        public bool Verified { get; set; }
    }
}
