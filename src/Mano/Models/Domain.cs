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

        [MaxLength(64)]
        [ForeignKey("Resume")]
        public string ResumeId { get; set; }

        public Resume Resume { get; set; }
    }
}
