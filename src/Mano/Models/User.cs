using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pomelo.AspNetCore.Extensions.BlobStorage.Models;

namespace Mano.Models
{
    public class User : IdentityUser<long>
    {
        public Sex Sex { get; set; }

        [MaxLength(32)]
        public string City { get; set; }

        [MaxLength(32)]
        public string Province { get; set; }

        [MaxLength(32)]
        public string GitHub { get; set; }

        [MaxLength(32)]
        public string GitOSC { get; set; }

        [MaxLength(32)]
        public string GitCSDN { get; set; }

        [MaxLength(32)]
        public string CodePlex { get; set; }

        [MaxLength(32)]
        public string CodingNet { get; set; }

        public string Introduction { get; set; }

        [MaxLength(64)]
        public string Address { get; set; }

        [MaxLength(16)]
        public string QQ { get; set; }

        [MaxLength(16)]
        public string WeChat { get; set; }

        public DateTime Birthday { get; set; }

        [MaxLength(18)]
        public string PRCID { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string BlogUrl { get; set; }

        [MaxLength(64)]
        public string Diploma { get; set; }

        [MaxLength(64)]
        public string Position { get; set; }

        [ForeignKey("Avatar")]
        public Guid? AvatarId { get; set; }

        public Blob Avatar { get; set; }

        public string LinkedIn { get; set; }

        public double Lon { get; set; }

        public double Lat { get; set; }

        [MaxLength(32)]
        public string Template { get; set; }

        public DateTime RegisteryTime { get; set; }

        public DateTime LastPullTime { get; set; }

        public string Statistics { get; set; } = "[]";

        public virtual ICollection<Domain> Domains { get; set; } = new List<Domain>();

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

        public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

        public virtual ICollection<Email> Emails { get; set; } = new List<Email>();
    }
}
