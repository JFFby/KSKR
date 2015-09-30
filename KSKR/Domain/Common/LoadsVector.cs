using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NCalc;

namespace Domain.Common
{
    public class LoadsVector
    {
        private string[] vector;

        public LoadsVector(string[] vector)
        {
            this.vector = vector;
        }

        public Vector<double> ToVector(double t)
        {
            var values = new List<double>();
            for (int i = 0; i < vector.Length; i++)
            {
                var expr = new Expression(vector[i], EvaluateOptions.IgnoreCase);
                if (vector[i].Contains("[t]"))
                {
                    expr.Parameters["t"] = t;
                }

                values.Add((double)expr.Evaluate());
            }

            return DenseVector.OfArray(values.ToArray());
        }
    }
}
