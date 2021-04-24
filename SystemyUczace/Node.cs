using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemyUczace
{
    public class Node
    {
        public Node()
        {
            node = new List<Node>();
            value = new List<string>();
        }

        public List<Node> node { get; set; }
        public int index { get; set; }
        public List<string> value { get; set; }
        public string decision { get; set; }
        public double gr { get; set; }
        public string[][] data { get; set; }
    }
}
