using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public Vector<double> ToVector(double t = 0)
        {
            var values = new List<double>();
            for (int i = 0; i < vector.Length; i++)
            {
                var fn = PrepareParameters(vector[i]);
                var expr = new Expression(fn, EvaluateOptions.IgnoreCase);
                if (fn.Contains("[t]"))
                {
                    expr.Parameters["t"] = t;
                }

                values.Add((double)expr.Evaluate());
            }

            return DenseVector.OfArray(values.ToArray());
        }

        private string PrepareParameters(string s)
        {
            const string pattern = @"(?<![.\d])(\d+)(?![.\d])";
            if (Regex.IsMatch(s, pattern))
            {
                s = Regex.Replace(s, pattern, x => x.Groups[1].Value + ".0");
            }

            return s;
        }
    }
}
