using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mano.Parser.Models
{
    public class Change
    {
        public Guid Id { get; set; }

        public string Path { get; set; }

        public long Additions { get; set; }

        public long Deletions { get; set; }

        [ForeignKey("Commit")]
        public Guid CommitId { get; set; }

        public Commit Commit { get; set; }
    }
}
