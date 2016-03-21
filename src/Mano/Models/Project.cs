using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Tilte { get; set; }

        public CommunityType Type { get; set; }

        [MaxLength(256)]
        public string ThirdPartyUrl { get; set; }

        [MaxLength(256)]
        public string ProjectUrl { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        [ForeignKey("Resume")]
        public long ResumeId { get; set; }

        public Resume Resume { get; set; }

        public double Size { get; set; }   // MiB

        [ForeignKey("Node")]
        public Guid? NodeId { get; set; }

        public Node Node { get; set; }

        public DateTime LastPullTime { get; set; }

        public DateTime LastEditTime { get; set; }
    }
}
