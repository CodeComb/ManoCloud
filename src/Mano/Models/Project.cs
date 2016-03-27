using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mano.Parser.Models;

namespace Mano.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        public CommunityType Type { get; set; }

        [MaxLength(256)]
        public string ThirdPartyUrl { get; set; }

        [MaxLength(256)]
        public string ProjectUrl { get; set; }

        [MaxLength(32)]
        public string Position { get; set; }

        public DateTime Begin { get; set; }

        public DateTime? End { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }

        public double Size { get; set; }   // MiB

        [ForeignKey("Node")]
        public Guid? NodeId { get; set; }

        public Node Node { get; set; }

        public DateTime LastPullTime { get; set; }

        public DateTime LastEditTime { get; set; }

        public string Hint { get; set; }

        public bool Verified { get; set; }

        public string Statistics { get; set; } = "[]";

        public bool IsContributed { get; set; }
    }
}
