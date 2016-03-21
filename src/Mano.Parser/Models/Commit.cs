using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Mano.Parser.Models
{
    public class Commit
    {
        [MaxLength(40)]
        public string Id { get; set; }

        [MaxLength(64)]
        public string Author { get; set; }

        [MaxLength(64)]
        public string Email { get; set; }

        public DateTime Time { get; set; }
        
        public virtual ICollection<Change> Changes { get; set; } = new List<Change>();
    }
}
