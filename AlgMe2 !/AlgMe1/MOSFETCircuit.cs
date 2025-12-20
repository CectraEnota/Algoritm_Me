using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class MOSFETInverterSimulator
    {
        private double R;     
        private double Rsi;     
        private double Czi;    
        private double Czs;    
        private double Csi;   
        private double Cn;    
        private double S;    
        private double Uп;    
        private double Uzi; 

        public MOSFETInverterSimulator(double r, double rsi, double czi, double czs,
                                      double csi, double cn, double s, double up, double uzi)
        {
            R = r;
            Rsi = rsi;
            Czi = czi;
            Czs = czs;
            Csi = csi;
            Cn = cn;
            S = s;
            Uп = up;
            Uzi = uzi;
        }

        public List<SimulationPoint> Simulate(double totalTime, double timeStep)
        {
            var results = new List<SimulationPoint>();

            double Ucsi = Uп;

            double Ceq = Czs * Cn / (Czs * Csi * Cn + Cn + Czs);
            double Req_inv = 1.0 / Rsi + 1.0 / R;

            int steps = (int)(totalTime / timeStep);

            for (int n = 0; n <= steps; n++)
            {
                double t = n * timeStep;

                double Ucn = Ucsi;

                var state = new double[] { Ucsi };
                var output = new double[] { Ucn };

                results.Add(new SimulationPoint(t, state, output));

                double dUcsi_dt = Ceq * (-Req_inv * Ucsi + (1.0 / R - S) * Uп * Uzi);

                Ucsi = Ucsi + timeStep * dUcsi_dt;
            }

            return results;
        }

        public double CalculatePropagationDelay(double thresholdVoltage)
        {
            double Ceq = Czs * Cn / (Czs * Csi * Cn + Cn + Czs);
            double Req_inv = 1.0 / Rsi + 1.0 / R;
            double tau = 1.0 / (Ceq * Req_inv);

            double tp = -tau * Math.Log((thresholdVoltage - Uп * (1.0 / R - S) / Req_inv) / Uп);

            return tp;
        }
    }
}
