using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Mano.Parser.Models
{
    public class Change
    {
        public Guid Id { get; set; }

        public string Path { get; set; }

        public long Additions { get; set; }

        public long Deletions { get; set; }

        [MaxLength(40)]
        [ForeignKey("Commit")]
        public string CommitId { get; set; }

        [JsonIgnore]
        public virtual Commit Commit { get; set; }
    }
}
