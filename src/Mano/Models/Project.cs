using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Tilte { get; set; }

        public CommunityType Type { get; set; }

        [MaxLength(256)]
        public string ThirdPartyUrl { get; set; }

        [MaxLength(256)]
        public string ProjectUrl { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();
    }
}
