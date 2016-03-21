using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Parser.Models
{
    public class Change
    {
        public string Path { get; set; }

        public long Additions { get; set; }

        public long Deletions { get; set; }
    }
}
