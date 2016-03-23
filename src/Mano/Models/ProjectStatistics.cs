using System;

namespace Mano.Models
{
    public class ProjectStatistics
    {
        public string Title { get; set; }
        public long Count { get; set; } 
        public long Contributed { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int Contributors { get; set; }
    }
}
