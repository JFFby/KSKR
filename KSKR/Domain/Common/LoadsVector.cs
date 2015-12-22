using System.Collections.Generic;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NCalc;

namespace Domain.Common
{

    public class LoadsVector
    {
        public LoadsVector(string[] vector)
        {
            this.Vector = vector;
        }


        public string[] Vector { get; private set; }

        public Vector<double> ToVector(double t = 0)
        {
            var values = new List<double>();
            for (int i = 0; i < Vector.Length; i++)
            {
                var fn = PrepareParameters(Vector[i]);
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
            const string replaceDelimiter = ",";
            if (Regex.IsMatch(s, pattern))
            {
                s = Regex.Replace(s, replaceDelimiter, ".");
            }

            if (Regex.IsMatch(s, pattern))
            {
                s = Regex.Replace(s, pattern, x => x.Groups[1].Value + ".0");
            }

            return s;
        }
    }
}
