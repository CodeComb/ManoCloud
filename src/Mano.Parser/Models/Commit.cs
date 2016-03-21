using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Parser.Models
{
    public class Commit
    {
        public string Author { get; set; }

        public string Email { get; set; }

        public DateTime Time { get; set; }

        public string Hash { get; set; }

        public List<Change> Changes { get; set; } = new List<Change>();
    }
}
