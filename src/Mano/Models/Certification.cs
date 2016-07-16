using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Mano.Models
{
    public class Certification
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        [MaxLength(64)]
        public string Number { get; set; }

        [ForeignKey("Cert")]
        public Guid? CertId { get; set; }

        public Blob Cert { get; set; }

        public DateTime AchieveTime { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual User User { get; set; }
    }
}
