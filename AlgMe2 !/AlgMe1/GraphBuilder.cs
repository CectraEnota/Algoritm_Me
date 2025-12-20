using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class GraphBuilder
    {
        public List<Edge> Edges { get; private set; }
        public HashSet<int> Nodes { get; private set; }
        public List<Edge> TreeEdges { get; private set; }
        public List<Edge> ChordEdges { get; private set; }

        public GraphBuilder()
        {
            Edges = new List<Edge>();
            Nodes = new HashSet<int>();
            TreeEdges = new List<Edge>();
            ChordEdges = new List<Edge>();
        }

        public void AddComponent(Component component)
        {
            Nodes.Add(component.Node1);
            Nodes.Add(component.Node2);

            if (component.Type == ComponentType.VCCS ||
                component.Type == ComponentType.VCVS ||
                component.Type == ComponentType.CCCS ||
                component.Type == ComponentType.CCVS)
            {
                if (component.ControlNode1 >= 0)
                    Nodes.Add(component.ControlNode1);
                if (component.ControlNode2 >= 0)
                    Nodes.Add(component.ControlNode2);
            }

            Edges.Add(new Edge(component.Node1, component.Node2, component));
        }


        public void BuildSpanningTree()
        {
            var priorityOrder = new Dictionary<ComponentType, int>
    {
        { ComponentType.VoltageSource, 1 },
        { ComponentType.Capacitor, 2 },
        { ComponentType.Resistor, 3 },
        { ComponentType.Inductor, 4 },
        { ComponentType.CurrentSource, 5 },
        { ComponentType.VCCS, 5 },
        { ComponentType.VCVS, 1 },
        { ComponentType.CCCS, 5 },
        { ComponentType.CCVS, 1 }
    };

            var sortedEdges = Edges.OrderBy(e =>
            {
                int priority;
                if (priorityOrder.TryGetValue(e.Component.Type, out priority))
                    return priority;
                return 6;
            }).ToList();

            var visitedNodes = new HashSet<int>();

            if (Nodes.Contains(0))
                visitedNodes.Add(0);
            else
                visitedNodes.Add(Nodes.First());

            bool added;
            do
            {
                added = false;
                foreach (var edge in sortedEdges)
                {
                    if (edge.IsInTree) continue;

                    bool node1Visited = visitedNodes.Contains(edge.From);
                    bool node2Visited = visitedNodes.Contains(edge.To);

                    if (node1Visited && !node2Visited)
                    {
                        edge.IsInTree = true;
                        TreeEdges.Add(edge);
                        visitedNodes.Add(edge.To);
                        added = true;
                    }
                    else if (!node1Visited && node2Visited)
                    {
                        edge.IsInTree = true;
                        edge.Direction = -1;
                        TreeEdges.Add(edge);
                        visitedNodes.Add(edge.From);
                        added = true;
                    }
                }
            } while (added && visitedNodes.Count < Nodes.Count);

            ChordEdges = Edges.Where(e => !e.IsInTree).ToList();

            ChordEdges = ChordEdges.OrderBy(e =>
            {
                int priority;
                if (priorityOrder.TryGetValue(e.Component.Type, out priority))
                    return -priority;
                return -6;
            }).ToList();
        }

        public string GetTreeInfo()
        {
            string info = "=== ДЕРЕВО ГРАФА ===\n";
            info += $"Всего узлов: {Nodes.Count}\n";
            info += $"Узлы: {string.Join(", ", Nodes.OrderBy(n => n))}\n\n";

            info += "Ветви дерева:\n";
            foreach (var edge in TreeEdges)
            {
                info += $"  {edge.Component.Name} ({edge.Component.Type}): {edge.From}->{edge.To}";
                if (edge.Component.Type == ComponentType.VCCS)
                {
                    info += $" [управление: {edge.Component.ControlNode1}->{edge.Component.ControlNode2}]";
                }
                info += "\n";
            }

            info += "\nХорды (дополнения дерева):\n";
            foreach (var edge in ChordEdges)
            {
                info += $"  {edge.Component.Name} ({edge.Component.Type}): {edge.From}->{edge.To}";
                if (edge.Component.Type == ComponentType.VCCS)
                {
                    info += $" [управление: {edge.Component.ControlNode1}->{edge.Component.ControlNode2}]";
                }
                info += "\n";
            }

            return info;
        }
    }
}
