using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Email
    {
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }

        [MaxLength(64)]
        public string EmailAddress { get; set; }

        public bool Verified { get; set; }
    }
}
