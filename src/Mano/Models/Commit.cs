using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Models
{
    public class Commit
    {
        public Guid Id { get; set; }

        public Guid CollaboratorId { get; set; }

        public Collaborator Collaborator { get; set; }

        public long Additions { get; set; }

        public long Deletions { get; set; }

        public long Commits { get; set; }

        public DateTime Time { get; set; }
    }
}
