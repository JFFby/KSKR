using System;
using Domain.Common;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Tests.Helpers
{
    public static class TestHelper
    {
        private const double Epselon = 0.01;

        public static Inputs GetTestInputData()
        {
            return new Inputs
            {
                M = DenseMatrix.OfArray(new double[,] { { 2, 0 }, { 0, 1 } }),
                Alpha = 0,
                Beta = 0,
                K = DenseMatrix.OfArray(new double[,] { { 6, -2 }, { -2, 4 } }),
                R = new LoadsVector(new[] { "0.0", "10.0" }),
                MovementU = DenseVector.OfArray(new double[] { 0, 0 }),
                SpeedU = DenseVector.OfArray(new double[] { 0, 0 }),
                T0 = 0,
                DeltaT = 0.28,
                Tk = 5
            };
        }

        public static void Assert(double expexted, double result, double epselon = Epselon)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Math.Abs(expexted - result) < epselon);
        }
    }
}
