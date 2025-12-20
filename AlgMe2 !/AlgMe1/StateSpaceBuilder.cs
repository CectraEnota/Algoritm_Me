using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class StateSpaceBuilder
    {
        private GraphBuilder graph;
        private List<Component> capacitors;
        private List<Component> inductors;
        private List<Component> voltageSources;
        private List<Component> currentSources;
        private List<Component> resistors;

        public int StateCount { get; private set; }
        public int InputCount { get; private set; }

        public double[,] MatrixA { get; private set; }
        public double[,] MatrixB { get; private set; }
        public double[,] MatrixC { get; private set; }
        public double[,] MatrixD { get; private set; }

        public List<string> StateVariables { get; private set; }
        public List<string> InputVariables { get; private set; }
        public List<string> OutputVariables { get; private set; }

        public StateSpaceBuilder(GraphBuilder graph)
        {
            this.graph = graph;
            StateVariables = new List<string>();
            InputVariables = new List<string>();
            OutputVariables = new List<string>();

            ClassifyComponents();
        }

        private void ClassifyComponents()
        {
            capacitors = graph.Edges.Where(e => e.Component.Type == ComponentType.Capacitor)
                                   .Select(e => e.Component).ToList();
            inductors = graph.Edges.Where(e => e.Component.Type == ComponentType.Inductor)
                                  .Select(e => e.Component).ToList();
            voltageSources = graph.Edges.Where(e => e.Component.Type == ComponentType.VoltageSource)
                                       .Select(e => e.Component).ToList();
            currentSources = graph.Edges.Where(e => e.Component.Type == ComponentType.CurrentSource)
                                       .Select(e => e.Component).ToList();
            resistors = graph.Edges.Where(e => e.Component.Type == ComponentType.Resistor)
                                  .Select(e => e.Component).ToList();

            foreach (var cap in capacitors)
                StateVariables.Add($"Uc_{cap.Name}");
            foreach (var ind in inductors)
                StateVariables.Add($"IL_{ind.Name}");

            StateCount = StateVariables.Count;

            foreach (var vs in voltageSources)
                InputVariables.Add($"V_{vs.Name}");
            foreach (var cs in currentSources)
                InputVariables.Add($"I_{cs.Name}");

            InputCount = Math.Max(InputVariables.Count, 1);
        }
        private void BuildMOSFETInverter()
        {
            if (capacitors.Count == 0)
            {
                return;
            }

            var loadResistor = resistors.FirstOrDefault(res =>
                res.Name.ToUpper().Contains("R1") || res.Name.ToUpper().StartsWith("R"));

            var allCapacitors = capacitors.ToList();

            var vccsSource = graph.Edges.Where(e => e.Component.Type == ComponentType.VCCS)
                                 .Select(e => e.Component).FirstOrDefault();

            if (loadResistor == null || vccsSource == null || allCapacitors.Count == 0)
            {
                BuildGeneralCircuitImproved();
                return;
            }

            int outputNode = vccsSource.Node2;

            double totalCapacitance = 0;
            foreach (var cap in allCapacitors)
            {
                if (cap.Node1 == outputNode || cap.Node2 == outputNode)
                {
                    totalCapacitance += cap.Value;
                }
            }

            if (totalCapacitance == 0)
            {
                BuildGeneralCircuitImproved();
                return;
            }

            StateVariables.Clear();
            StateVariables.Add($"Uout_node{outputNode}");
            StateCount = 1;

            double resistanceLoad = loadResistor.Value;
            double transconductance = vccsSource.Gain;

            MatrixA = new double[1, 1];
            MatrixB = new double[1, InputCount];

            MatrixA[0, 0] = -1.0 / (resistanceLoad * totalCapacitance);

            int vddIndex = -1;
            for (int i = 0; i < voltageSources.Count; i++)
            {
                var vs = voltageSources[i];
                if (vs.Name.ToUpper().Contains("VDD") || vs.Name.ToUpper().Contains("DD"))
                {
                    vddIndex = i;
                    break;
                }
            }

            if (vddIndex >= 0 && vddIndex < InputCount)
            {
                MatrixB[0, vddIndex] = 1.0 / (resistanceLoad * totalCapacitance);
            }

            int vinIndex = -1;
            for (int i = 0; i < voltageSources.Count; i++)
            {
                var vs = voltageSources[i];
                if (vs.Name.ToUpper().Contains("VIN") || vs.Name.ToUpper().Contains("IN"))
                {
                    vinIndex = i;
                    break;
                }
            }

            if (vinIndex >= 0 && vinIndex < InputCount)
            {
                MatrixB[0, vinIndex] = -transconductance / totalCapacitance;
            }

            double timeConstant = resistanceLoad * totalCapacitance;

            if (vddIndex >= 0 && vddIndex < voltageSources.Count)
            {
                double vdd = voltageSources[vddIndex].Value;
                double vthreshold = vdd / 2.0;
                double tpd = -timeConstant * Math.Log(vthreshold / vdd);
            }
        }

        public void BuildStateSpaceMatrices()
        {
            MatrixA = new double[StateCount, StateCount];
            MatrixB = new double[StateCount, InputCount];

            BuildMatricesCorrect();

            OutputVariables.Clear();

            int actualStateCount = Math.Min(StateVariables.Count, StateCount);
            for (int i = 0; i < actualStateCount; i++)
            {
                OutputVariables.Add(StateVariables[i]);
            }

            int outputCount = OutputVariables.Count;

            if (outputCount > 0)
            {
                MatrixC = new double[outputCount, StateCount];
                MatrixD = new double[outputCount, InputCount];

                for (int i = 0; i < outputCount && i < StateCount; i++)
                    MatrixC[i, i] = 1.0;
            }
        }

        private void BuildMatricesCorrect()
        {
            var topology = AnalyzeTopology();

            if (topology == CircuitTopology.MOSFETInverter)
            {
                BuildMOSFETInverter();
            }
            else if (topology == CircuitTopology.SeriesRLC)
            {
                BuildSeriesRLC();
            }
            else if (topology == CircuitTopology.ParallelRLC)
            {
                BuildParallelRLC();
            }
            else if (topology == CircuitTopology.CascadeRC)
            {
                BuildCascadeRC();
            }
            else if (topology == CircuitTopology.SeriesRC)
            {
                BuildSeriesRC();
            }
            else if (topology == CircuitTopology.SeriesRL)
            {
                BuildSeriesRL();
            }
            else if (topology == CircuitTopology.SeriesLC)
            {
                BuildSeriesLC();
            }
            else
            {
                BuildGeneralCircuitImproved();
            }
        }
        private void BuildCascadeRC()
        {
            int numCaps = capacitors.Count;
            if (numCaps == 0) return;

            var nodeToCapacitor = new Dictionary<int, Component>();
            foreach (var cap in capacitors)
            {
                int node = (cap.Node1 != 0) ? cap.Node1 : cap.Node2;
                nodeToCapacitor[node] = cap;
            }

            for (int i = 0; i < numCaps; i++)
            {
                var cap = capacitors[i];
                int capNode = (cap.Node1 != 0) ? cap.Node1 : cap.Node2;
                double C = cap.Value;

                var connectedResistors = resistors.Where(r =>
                    r.Node1 == capNode || r.Node2 == capNode).ToList();

                foreach (var res in connectedResistors)
                {
                    int otherNode = (res.Node1 == capNode) ? res.Node2 : res.Node1;
                    double R = res.Value;

                    if (otherNode == 0)
                    {
                        continue;
                    }
                    else if (nodeToCapacitor.ContainsKey(otherNode))
                    {
                        int otherCapIdx = capacitors.IndexOf(nodeToCapacitor[otherNode]);

                        MatrixA[i, i] -= 1.0 / (R * C); 
                        MatrixA[i, otherCapIdx] += 1.0 / (R * C); 
                    }
                    else
                    {
                        var vs = voltageSources.FirstOrDefault(v =>
                            v.Node1 == otherNode || v.Node2 == otherNode);

                        if (vs != null)
                        {
                            int vsIdx = voltageSources.IndexOf(vs);
                            MatrixB[i, vsIdx] = 1.0 / (R * C);
                            MatrixA[i, i] -= 1.0 / (R * C);
                        }
                    }
                }
            }
        }

        private CircuitTopology AnalyzeTopology()
        {
            bool hasR = resistors.Count > 0;
            bool hasL = inductors.Count > 0;
            bool hasC = capacitors.Count > 0;

            bool hasVCCS = graph.Edges.Any(e => e.Component.Type == ComponentType.VCCS);

            if (hasVCCS && hasC && hasR)
            {
                return CircuitTopology.MOSFETInverter;
            }

            bool isSeries = IsSeriesConnection();

            if (isSeries && hasR && hasL && hasC)
                return CircuitTopology.SeriesRLC;
            else if (hasR && hasL && hasC)
                return CircuitTopology.ParallelRLC;
            else if (hasR && hasC && capacitors.Count > 1)
                return CircuitTopology.CascadeRC;
            else if (hasR && hasC)
                return CircuitTopology.SeriesRC;
            else if (hasR && hasL)
                return CircuitTopology.SeriesRL;
            else if (hasL && hasC)
                return CircuitTopology.SeriesLC;
            else
                return CircuitTopology.General;
        }

        private bool IsSeriesConnection()
        {
            var allComponents = new List<Component>();
            allComponents.AddRange(resistors);
            allComponents.AddRange(inductors);
            allComponents.AddRange(capacitors);
            allComponents.AddRange(voltageSources);
            allComponents.AddRange(currentSources);

            var nodeCounts = new Dictionary<int, int>();
            foreach (var comp in allComponents)
            {
                if (!nodeCounts.ContainsKey(comp.Node1))
                    nodeCounts[comp.Node1] = 0;
                if (!nodeCounts.ContainsKey(comp.Node2))
                    nodeCounts[comp.Node2] = 0;
                nodeCounts[comp.Node1]++;
                nodeCounts[comp.Node2]++;
            }

            int twoConnectionNodes = nodeCounts.Values.Count(c => c == 2);
            return twoConnectionNodes >= allComponents.Count - 1;
        }

        private void BuildSeriesRLC()
        {
            if (capacitors.Count == 0 || inductors.Count == 0)
                return;

            var C = capacitors[0].Value;
            var L = inductors[0].Value;
            var R = resistors.Count > 0 ? resistors.Sum(r => r.Value) : 0;

            MatrixA[0, 0] = 0;                   
            if (StateCount > 1)
                MatrixA[0, 1] = 1.0 / C;         

            if (StateCount > 1)
            {
                MatrixA[1, 0] = -1.0 / L;       
                MatrixA[1, 1] = -R / L;       
            }

            if (voltageSources.Count > 0)
            {
                if (StateCount > 1)
                    MatrixB[1, 0] = 1.0 / L;
            }
            if (currentSources.Count > 0)
            {
                int idx = voltageSources.Count;
                if (idx < InputCount)
                    MatrixB[0, idx] = 1.0 / C;
            }
        }

        private void BuildSeriesRC()
        {
            if (capacitors.Count == 0)
                return;

            var C = capacitors[0].Value;
            var R = resistors.Sum(r => r.Value);

            MatrixA[0, 0] = -1.0 / (R * C);

            if (voltageSources.Count > 0)
                MatrixB[0, 0] = 1.0 / (R * C);
            if (currentSources.Count > 0)
            {
                int idx = voltageSources.Count;
                if (idx < InputCount)
                    MatrixB[0, idx] = 1.0 / C;
            }
        }

        private void BuildSeriesRL()
        {
            if (inductors.Count == 0)
                return;

            var L = inductors[0].Value;
            var R = resistors.Sum(r => r.Value);

            MatrixA[0, 0] = -R / L;

            if (voltageSources.Count > 0)
                MatrixB[0, 0] = 1.0 / L;
            if (currentSources.Count > 0)
            {
                int idx = voltageSources.Count;
                if (idx < InputCount)
                    MatrixB[0, idx] = R;
            }
        }

        private void BuildSeriesLC()
        {
            if (capacitors.Count == 0 || inductors.Count == 0)
                return;

            var C = capacitors[0].Value;
            var L = inductors[0].Value;

            MatrixA[0, 0] = 0;
            MatrixA[0, 1] = 1.0 / C;
            MatrixA[1, 0] = -1.0 / L;
            MatrixA[1, 1] = 0;

            if (voltageSources.Count > 0)
                MatrixB[1, 0] = 1.0 / L;
            if (currentSources.Count > 0)
            {
                int idx = voltageSources.Count;
                if (idx < InputCount)
                {
                    MatrixB[0, idx] = 1.0 / C;
                    MatrixB[1, idx] = 0;
                }
            }
        }

        private void BuildParallelRLC()
        {
            if (capacitors.Count == 0)
                return;

            var C = capacitors[0].Value;
            var L = inductors.Count > 0 ? inductors[0].Value : double.MaxValue;
            var R = resistors.Count > 0 ? resistors[0].Value : double.MaxValue;

            if (inductors.Count > 0)
            {
                MatrixA[0, 0] = -1.0 / (R * C);
                MatrixA[0, 1] = -1.0 / C;
                MatrixA[1, 0] = 1.0 / L;
                MatrixA[1, 1] = 0;

                if (currentSources.Count > 0)
                {
                    int idx = voltageSources.Count;
                    if (idx < InputCount)
                        MatrixB[0, idx] = 1.0 / C;
                }
            }
        }

        private void BuildGeneralCircuit()
        {
            for (int i = 0; i < capacitors.Count; i++)
            {
                var cap = capacitors[i];
                foreach (var cs in currentSources)
                {
                    int idx = voltageSources.Count + currentSources.IndexOf(cs);
                    if (idx < InputCount)
                        MatrixB[i, idx] = 1.0 / cap.Value;
                }
            }

            for (int i = 0; i < inductors.Count; i++)
            {
                var ind = inductors[i];
                int row = capacitors.Count + i;
                if (row >= StateCount) break;

                foreach (var vs in voltageSources)
                {
                    int idx = voltageSources.IndexOf(vs);
                    MatrixB[row, idx] = 1.0 / ind.Value;
                }
            }
        }
        private void BuildGeneralCircuitImproved()
        {
            var nodes = new HashSet<int>();
            foreach (var comp in graph.Edges)
            {
                if (comp.Component.Node1 != 0) nodes.Add(comp.Component.Node1);
                if (comp.Component.Node2 != 0) nodes.Add(comp.Component.Node2);
            }

            var nodeList = nodes.OrderBy(n => n).ToList();

            var nodeToCapacitor = new Dictionary<int, int>();
            for (int i = 0; i < capacitors.Count; i++)
            {
                var cap = capacitors[i];
                int node = (cap.Node1 != 0) ? cap.Node1 : cap.Node2;
                nodeToCapacitor[node] = i;
            }

            var nodeToVoltageSource = new Dictionary<int, int>();
            for (int i = 0; i < voltageSources.Count; i++)
            {
                var vs = voltageSources[i];
                int node = (vs.Node1 != 0) ? vs.Node1 : vs.Node2;
                nodeToVoltageSource[node] = i;
            }

            for (int i = 0; i < capacitors.Count; i++)
            {
                var cap = capacitors[i];
                int capNode = (cap.Node1 != 0) ? cap.Node1 : cap.Node2;
                double C = cap.Value;

                foreach (var res in resistors)
                {
                    if (res.Node1 == capNode || res.Node2 == capNode)
                    {
                        int otherNode = (res.Node1 == capNode) ? res.Node2 : res.Node1;
                        double R = res.Value;

                        double sign = (res.Node1 == capNode) ? -1.0 : 1.0;

                        if (otherNode == 0)
                        {
                            MatrixA[i, i] -= 1.0 / (R * C);
                        }
                        else if (nodeToCapacitor.ContainsKey(otherNode))
                        {
                            int otherCapIdx = nodeToCapacitor[otherNode];
                            MatrixA[i, otherCapIdx] += sign / (R * C);
                            MatrixA[i, i] -= sign / (R * C);
                        }
                        else if (nodeToVoltageSource.ContainsKey(otherNode))
                        {
                            int vsIdx = nodeToVoltageSource[otherNode];
                            MatrixB[i, vsIdx] = sign / (R * C);
                            MatrixA[i, i] -= sign / (R * C);
                        }
                        else
                        {
                            var voltageAtNode = FindVoltageAtNode(otherNode, nodeToCapacitor, nodeToVoltageSource);

                            if (voltageAtNode.IsCapacitor)
                            {
                                MatrixA[i, voltageAtNode.Index] += sign / (R * C);
                                MatrixA[i, i] -= sign / (R * C);
                            }
                            else if (voltageAtNode.IsVoltageSource)
                            {
                                MatrixB[i, voltageAtNode.Index] = sign / (R * C);
                                MatrixA[i, i] -= sign / (R * C);
                            }
                        }
                    }
                }

                foreach (var ind in inductors)
                {
                    if (ind.Node1 == capNode || ind.Node2 == capNode)
                    {
                        int indIdx = inductors.IndexOf(ind);
                        int stateIdx = capacitors.Count + indIdx;

                        double sign = (ind.Node2 == capNode) ? 1.0 : -1.0;

                        MatrixA[i, stateIdx] = sign / C;
                    }
                }

                foreach (var cs in currentSources)
                {
                    if (cs.Node1 == capNode || cs.Node2 == capNode)
                    {
                        int csIdx = voltageSources.Count + currentSources.IndexOf(cs);

                        double sign = (cs.Node2 == capNode) ? 1.0 : -1.0;

                        if (csIdx < InputCount)
                            MatrixB[i, csIdx] = sign / C;
                    }
                }
            }

            for (int i = 0; i < inductors.Count; i++)
            {
                var ind = inductors[i];
                int row = capacitors.Count + i;
                double L = ind.Value;

                int node1 = ind.Node1;
                int node2 = ind.Node2;

                double[] node1Contribution = GetNodePotentialContribution(node1, nodeToCapacitor,
                                                                           nodeToVoltageSource, L);
                double[] node2Contribution = GetNodePotentialContribution(node2, nodeToCapacitor,
                                                                           nodeToVoltageSource, L);

                for (int j = 0; j < StateCount; j++)
                {
                    MatrixA[row, j] = (node1Contribution[j] - node2Contribution[j]) / L;
                }

                for (int j = 0; j < InputCount; j++)
                {
                    double b1 = (j < node1Contribution.Length - StateCount) ?
                                node1Contribution[StateCount + j] : 0;
                    double b2 = (j < node2Contribution.Length - StateCount) ?
                                node2Contribution[StateCount + j] : 0;
                    MatrixB[row, j] = (b1 - b2) / L;
                }

                var seriesResistors = FindSeriesResistors(ind);
                double totalSeriesR = seriesResistors.Sum(r => r.Value);

                if (totalSeriesR > 0)
                {
                    MatrixA[row, row] -= totalSeriesR / L;
                }
            }
        }

        private struct NodeVoltage
        {
            public bool IsCapacitor;
            public bool IsVoltageSource;
            public bool IsGround;
            public int Index;
        }

        private NodeVoltage FindVoltageAtNode(int node, Dictionary<int, int> nodeToCapacitor,
                                             Dictionary<int, int> nodeToVoltageSource)
        {
            if (node == 0)
                return new NodeVoltage { IsGround = true };

            if (nodeToCapacitor.ContainsKey(node))
                return new NodeVoltage { IsCapacitor = true, Index = nodeToCapacitor[node] };

            if (nodeToVoltageSource.ContainsKey(node))
                return new NodeVoltage { IsVoltageSource = true, Index = nodeToVoltageSource[node] };

            foreach (var res in resistors)
            {
                if (res.Node1 == node)
                {
                    return FindVoltageAtNode(res.Node2, nodeToCapacitor, nodeToVoltageSource);
                }
                else if (res.Node2 == node)
                {
                    return FindVoltageAtNode(res.Node1, nodeToCapacitor, nodeToVoltageSource);
                }
            }

            return new NodeVoltage { IsGround = true };
        }

        private double[] GetNodePotentialContribution(int node, Dictionary<int, int> nodeToCapacitor,
                                                     Dictionary<int, int> nodeToVoltageSource, double L)
        {
            double[] result = new double[StateCount + InputCount];

            if (node == 0)
            {
                return result;
            }

            if (nodeToCapacitor.ContainsKey(node))
            {
                int capIdx = nodeToCapacitor[node];
                result[capIdx] = 1.0;
                return result;
            }

            if (nodeToVoltageSource.ContainsKey(node))
            {
                int vsIdx = nodeToVoltageSource[node];
                result[StateCount + vsIdx] = 1.0;
                return result;
            }

            var connectedResistors = resistors.Where(r => r.Node1 == node || r.Node2 == node).ToList();

            if (connectedResistors.Count > 0)
            {
                var res = connectedResistors[0];
                int otherNode = (res.Node1 == node) ? res.Node2 : res.Node1;

                var otherPotential = GetNodePotentialContribution(otherNode, nodeToCapacitor,
                                                                  nodeToVoltageSource, L);

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = otherPotential[i];
                }
            }

            return result;
        }

        private List<Component> FindSeriesResistors(Component inductor)
        {
            var seriesResistors = new List<Component>();

            foreach (var res in resistors)
            {
                if ((res.Node1 == inductor.Node1 || res.Node1 == inductor.Node2 ||
                     res.Node2 == inductor.Node1 || res.Node2 == inductor.Node2))
                {
                    if (IsSeriesBetween(inductor, res))
                    {
                        seriesResistors.Add(res);
                    }
                }
            }
            return seriesResistors;
        }

        private bool IsSeriesBetween(Component comp1, Component comp2)
        {
            int commonNode = -1;
            if (comp1.Node1 == comp2.Node1 || comp1.Node1 == comp2.Node2)
                commonNode = comp1.Node1;
            else if (comp1.Node2 == comp2.Node1 || comp1.Node2 == comp2.Node2)
                commonNode = comp1.Node2;

            if (commonNode == -1 || commonNode == 0)
                return false;

            int connectionCount = 0;
            foreach (var edge in graph.Edges)
            {
                if (edge.Component.Node1 == commonNode || edge.Component.Node2 == commonNode)
                    connectionCount++;
            }

            return connectionCount == 2;
        }

        private void BuildInductorEquationsSimplified()
        {
            for (int i = 0; i < inductors.Count; i++)
            {
                var ind = inductors[i];
                int row = capacitors.Count + i;
                double L = ind.Value;

                int node1 = ind.Node1;
                int node2 = ind.Node2;

                if (IsSeriesRLCTopology(ind))
                {
                    var cap = capacitors.FirstOrDefault(c =>
                        c.Node1 == node2 || c.Node2 == node2 ||
                        c.Node1 == node1 || c.Node2 == node1);

                    if (cap != null)
                    {
                        int capIdx = capacitors.IndexOf(cap);
                        MatrixA[row, capIdx] = -1.0 / L;
                    }

                    var res = resistors.FirstOrDefault(r =>
                        r.Node1 == node1 || r.Node2 == node1 ||
                        r.Node1 == node2 || r.Node2 == node2);

                    if (res != null)
                    {
                        MatrixA[row, row] = -res.Value / L;
                    }

                    if (voltageSources.Count > 0)
                    {
                        MatrixB[row, 0] = 1.0 / L;
                    }
                }
                else if (IsParallelRLCTopology(ind))
                {
                    var cap = capacitors.FirstOrDefault(c =>
                        (c.Node1 == node1 && c.Node2 == node2) ||
                        (c.Node1 == node2 && c.Node2 == node1) ||
                        (c.Node1 == node1 || c.Node2 == node1));

                    if (cap != null)
                    {
                        int capIdx = capacitors.IndexOf(cap);
                        MatrixA[row, capIdx] = 1.0 / L;
                    }
                }
                else
                {
                    BuildInductorEquationGeneral(ind, row, L);
                }
            }
        }

        private void BuildInductorEquationGeneral(Component inductor, int row, double L)
        {
            int node1 = inductor.Node1;
            int node2 = inductor.Node2;

            if (node1 == 0)
            {
            }
            else
            {
                var cap1 = capacitors.FirstOrDefault(c =>
                    (c.Node1 == node1 && c.Node2 == 0) ||
                    (c.Node2 == node1 && c.Node1 == 0));

                if (cap1 != null)
                {
                    int capIdx = capacitors.IndexOf(cap1);
                    MatrixA[row, capIdx] += 1.0 / L;
                }

                var vs1 = voltageSources.FirstOrDefault(v =>
                    (v.Node1 == node1 && v.Node2 == 0) ||
                    (v.Node2 == node1 && v.Node1 == 0));

                if (vs1 != null)
                {
                    int vsIdx = voltageSources.IndexOf(vs1);
                    MatrixB[row, vsIdx] += 1.0 / L;
                }
            }

            if (node2 == 0)
            {
            }
            else
            {
                var cap2 = capacitors.FirstOrDefault(c =>
                    (c.Node1 == node2 && c.Node2 == 0) ||
                    (c.Node2 == node2 && c.Node1 == 0));

                if (cap2 != null)
                {
                    int capIdx = capacitors.IndexOf(cap2);
                    MatrixA[row, capIdx] -= 1.0 / L;
                }

                var vs2 = voltageSources.FirstOrDefault(v =>
                    (v.Node1 == node2 && v.Node2 == 0) ||
                    (v.Node2 == node2 && v.Node1 == 0));

                if (vs2 != null)
                {
                    int vsIdx = voltageSources.IndexOf(vs2);
                    MatrixB[row, vsIdx] -= 1.0 / L;
                }
            }

            foreach (var res in resistors)
            {
                if ((res.Node1 == node1 || res.Node2 == node1 ||
                     res.Node1 == node2 || res.Node2 == node2) &&
                    IsSeriesBetween(inductor, res))
                {
                    MatrixA[row, row] -= res.Value / L;
                }
            }
        }

        private bool IsSeriesRLCTopology(Component inductor)
        {
            int node1 = inductor.Node1;
            int node2 = inductor.Node2;

            bool hasCapAtNode2 = capacitors.Any(c => c.Node1 == node2 || c.Node2 == node2);
            bool hasResAtNode1 = resistors.Any(r => r.Node1 == node1 || r.Node2 == node1);
            bool hasVoltageSource = voltageSources.Any();

            return hasCapAtNode2 && hasResAtNode1 && hasVoltageSource;
        }

        private bool IsParallelRLCTopology(Component inductor)
        {
            int node1 = inductor.Node1;
            int node2 = inductor.Node2;

            bool hasCapSameNodes = capacitors.Any(c =>
                (c.Node1 == node1 && c.Node2 == node2) ||
                (c.Node1 == node2 && c.Node2 == node1));

            return hasCapSameNodes;
        }

        public string GetMatrixInfo()
        {
            string info = "=== МАТРИЦЫ ПРОСТРАНСТВА СОСТОЯНИЙ ===\n\n";

            info += string.Format("Переменные состояния ({0}):\n", StateCount);

            int actualStateVars = Math.Min(StateVariables.Count, StateCount);
            for (int i = 0; i < actualStateVars; i++)
                info += string.Format("  x{0}: {1}\n", i + 1, StateVariables[i]);

            info += string.Format("\nВходные воздействия ({0}):\n", InputCount);

            int actualInputVars = Math.Min(InputVariables.Count, InputCount);
            for (int i = 0; i < actualInputVars; i++)
                info += string.Format("  u{0}: {1}\n", i + 1, InputVariables[i]);

            info += "\nМатрица A:\n";
            info += MatrixToString(MatrixA);

            info += "\nМатрица B:\n";
            info += MatrixToString(MatrixB);

            if (MatrixC != null)
            {
                info += "\nМатрица C:\n";
                info += MatrixToString(MatrixC);
            }

            info += "\nУравнения состояния:\n";

            int rowCount = MatrixA != null ? MatrixA.GetLength(0) : 0;

            for (int i = 0; i < rowCount && i < StateVariables.Count; i++)
            {
                info += string.Format("  d({0})/dt = ", StateVariables[i]);
                bool first = true;

                for (int j = 0; j < MatrixA.GetLength(1); j++)
                {
                    if (Math.Abs(MatrixA[i, j]) > 1e-10)
                    {
                        if (!first && MatrixA[i, j] > 0) info += " + ";
                        if (!first && MatrixA[i, j] < 0) info += " - ";
                        if (first && MatrixA[i, j] < 0) info += "-";

                        string varName = j < StateVariables.Count ? StateVariables[j] : string.Format("x{0}", j + 1);
                        info += string.Format("{0:G6}*{1}", Math.Abs(MatrixA[i, j]), varName);
                        first = false;
                    }
                }

                if (MatrixB != null && i < MatrixB.GetLength(0))
                {
                    for (int j = 0; j < MatrixB.GetLength(1) && j < InputVariables.Count; j++)
                    {
                        if (Math.Abs(MatrixB[i, j]) > 1e-10)
                        {
                            if (!first && MatrixB[i, j] > 0) info += " + ";
                            if (!first && MatrixB[i, j] < 0) info += " - ";
                            if (first && MatrixB[i, j] < 0) info += "-";

                            info += string.Format("{0:G6}*{1}", Math.Abs(MatrixB[i, j]), InputVariables[j]);
                            first = false;
                        }
                    }
                }

                if (first) info += "0";
                info += "\n";
            }

            return info;
        }

        private string MatrixToString(double[,] matrix)
        {
            if (matrix == null) return "null\n";

            string result = "";
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                result += "  [ ";
                for (int j = 0; j < cols; j++)
                {
                    result += string.Format("{0,12:G6} ", matrix[i, j]);
                }
                result += "]\n";
            }
            return result;
        }
    }

    public enum CircuitTopology
    {
        SeriesRLC,
        ParallelRLC,
        CascadeRC,
        SeriesRC,
        SeriesRL,
        SeriesLC,
        MOSFETInverter,
        General
    }
}
