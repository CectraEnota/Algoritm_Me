using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class Component
    {
        public string Name { get; set; }
        public ComponentType Type { get; set; }
        public int Node1 { get; set; }
        public int Node2 { get; set; }
        public double Value { get; set; }

        public int ControlNode1 { get; set; }
        public int ControlNode2 { get; set; }  
        public double Gain { get; set; } 

        public Component(string name, ComponentType type, int node1, int node2, double value)
        {
            Name = name;
            Type = type;
            Node1 = node1;
            Node2 = node2;
            Value = value;
            ControlNode1 = -1;
            ControlNode2 = -1;
            Gain = 0;
        }
        public Component(string name, ComponentType type, int node1, int node2,
                        int controlNode1, int controlNode2, double gain)
        {
            Name = name;
            Type = type;
            Node1 = node1;
            Node2 = node2;
            Value = 0;
            ControlNode1 = controlNode1;
            ControlNode2 = controlNode2;
            Gain = gain;
        }

        public override string ToString()
        {
            if (Type == ComponentType.VCCS || Type == ComponentType.VCVS ||
                Type == ComponentType.CCCS || Type == ComponentType.CCVS)
            {
                return $"{Type} {Name}: {Node1}->{Node2}, Control={ControlNode1}->{ControlNode2}, Gain={Gain}";
            }
            return $"{Type} {Name}: {Node1}->{Node2}, Value={Value}";
        }
    }
}
