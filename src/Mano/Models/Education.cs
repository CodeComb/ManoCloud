using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeComb.AspNet.Upload.Models;

namespace Mano.Models
{
    public class Education
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string School { get; set; }

        [MaxLength(128)]
        public string Profession { get; set; }

        public DateTime Begin { get; set; }

        public DateTime? End { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }

        [ForeignKey("Certificate")]
        public Guid? CertificateId { get; set; }

        public File Certificate { get; set; }

        public bool Verified { get; set; }

        public string Hint { get; set; }
    }
}
