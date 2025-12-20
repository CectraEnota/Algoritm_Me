using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class Edge
    {
        public int From { get; set; }
        public int To { get; set; }
        public Component Component { get; set; }
        public bool IsInTree { get; set; }
        public int Direction { get; set; }

        public Edge(int from, int to, Component component)
        {
            From = from;
            To = to;
            Component = component;
            IsInTree = false;
            Direction = 1;
        }
    }
}
