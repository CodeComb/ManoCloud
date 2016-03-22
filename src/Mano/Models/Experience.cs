using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeComb.AspNet.Upload.Models;

namespace Mano.Models
{
    public class Experience
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Company { get; set; }

        [MaxLength(64)]
        public string Position { get; set; }

        [MaxLength(512)]
        public string Hint { get; set; }

        public DateTime Begin { get; set; }

        public DateTime? End { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }

        public Guid? LaborContractId { get; set; }

        public File LaborContract { get; set; }

        public bool Verified { get; set; }
    }
}
