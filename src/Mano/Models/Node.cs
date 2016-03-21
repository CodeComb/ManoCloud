using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mano.Models
{
    public enum NodeStatus
    {
        Online,
        Offline
    }

    public class Node
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Url { get; set; }

        public double MaxSize { get; set; } = 10240; // MiB

        public NodeStatus Status { get; set; }
    }
}
