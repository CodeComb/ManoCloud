using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mano.Models
{
    public class User : IdentityUser<long>
    {
        public Sex Sex { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string GitHub { get; set; }

        public string GitOSC { get; set; }

        public string GitCSDN { get; set; }

        public string Introduction { get; set; }

        public string Address { get; set; }

        [MaxLength(18)]
        public string PRCID { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(32)]
        public string UrlKey { get; set; }

        public string BlogUrl { get; set; }

        [MaxLength(64)]
        public string Diploma { get; set; }

        public virtual ICollection<Domain> Domains { get; set; } = new List<Domain>();

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
