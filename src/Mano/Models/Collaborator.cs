using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Collaborator
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Identifier { get; set; }

        public CommunityType Community { get; set; }

        [MaxLength(256)]
        public string AvatarUrl { get; set; }

        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
