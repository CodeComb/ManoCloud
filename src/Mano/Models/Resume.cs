using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Mano.Models
{
    public class Resume
    {
        [MaxLength(64)]
        public string Id { get; set; }

        public string Introduction { get; set; }

        public string PRCID { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string Diploma { get; set; }

        public virtual ICollection<Domain> Domains { get; set; } = new List<Domain>();
    }
}
