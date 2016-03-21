using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mano.Models
{
    public class User : IdentityUser<long>
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string GitHub { get; set; }

        public string GitOSC { get; set; }

        public string GitCSDN { get; set; }
    }
}
