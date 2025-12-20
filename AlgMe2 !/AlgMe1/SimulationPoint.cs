using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class SimulationPoint
    {
        public double Time { get; set; }
        public double[] State { get; set; }
        public double[] Output { get; set; }

        public SimulationPoint(double time, double[] state, double[] output)
        {
            Time = time;
            State = (double[])state.Clone();
            Output = output != null ? (double[])output.Clone() : null;
        }
    }
}
