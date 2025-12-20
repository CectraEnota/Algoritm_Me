using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgMe1
{
    public class Simulator
    {
        private StateSpaceBuilder stateSpace;
        private List<double> inputValues;

        public Simulator(StateSpaceBuilder stateSpace, List<double> inputValues)
        {
            this.stateSpace = stateSpace;
            this.inputValues = inputValues;
        }

        public List<SimulationPoint> RunSimulation(double totalTime, double timeStep,
                                           double[] initialState = null)
        {
            var results = new List<SimulationPoint>();

            int n = stateSpace.StateCount;
            int m = stateSpace.InputCount;

            double[] x = initialState ?? new double[n];
            double[] u = inputValues.ToArray();

            if (u.Length < m)
            {
                Array.Resize(ref u, m);
            }

            int steps = (int)(totalTime / timeStep);

            for (int step = 0; step <= steps; step++)
            {
                double t = step * timeStep;

                double[] y = ComputeOutput(x, u);

                results.Add(new SimulationPoint(t, x, y));

                double[] k1 = ComputeDerivative(x, u);
                double[] x_k2 = AddVectors(x, MultiplyVector(k1, timeStep / 2));
                double[] k2 = ComputeDerivative(x_k2, u);
                double[] x_k3 = AddVectors(x, MultiplyVector(k2, timeStep / 2));
                double[] k3 = ComputeDerivative(x_k3, u);
                double[] x_k4 = AddVectors(x, MultiplyVector(k3, timeStep));
                double[] k4 = ComputeDerivative(x_k4, u);

                for (int i = 0; i < n; i++)
                {
                    x[i] += timeStep / 6.0 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]);
                }
            }

            return results;
        }

        private double[] ComputeDerivative(double[] x, double[] u)
        {
            int n = x.Length;
            int m = u.Length;
            double[] result = new double[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i] += stateSpace.MatrixA[i, j] * x[j];
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i] += stateSpace.MatrixB[i, j] * u[j];
                }
            }

            return result;
        }

        private double[] ComputeOutput(double[] x, double[] u)
        {
            if (stateSpace.MatrixC == null) return null;

            int k = stateSpace.MatrixC.GetLength(0);
            int n = x.Length;
            int m = u.Length;
            double[] result = new double[k];

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i] += stateSpace.MatrixC[i, j] * x[j];
                }
            }

            if (stateSpace.MatrixD != null)
            {
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        result[i] += stateSpace.MatrixD[i, j] * u[j];
                    }
                }
            }

            return result;
        }

        private double[] AddVectors(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];
            return result;
        }

        private double[] MultiplyVector(double[] v, double scalar)
        {
            double[] result = new double[v.Length];
            for (int i = 0; i < v.Length; i++)
                result[i] = v[i] * scalar;
            return result;
        }
    }
}
