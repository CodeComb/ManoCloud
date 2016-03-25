using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Mano.Parser.Models
{
    public class Commit
    {
        public Guid Id { get; set; }

        [MaxLength(40)]
        public string Hash { get; set; }

        [MaxLength(64)]
        public string Author { get; set; }

        [MaxLength(64)]
        public string Email { get; set; }

        public DateTime Time { get; set; }

        public string Path { get; set; }

        public long Additions { get; set; }

        public long Deletions { get; set; }

        public string Extension { get; set; }
    }
}
