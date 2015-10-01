using System;
using Domain.CentralDifference;
using Domain.Common;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CentralDifferenceTest
    {
        [TestMethod]
        public void Test1()
        {
            var inputs = new Inputs
            {
                M = DenseMatrix.OfArray(new double[,] {{2, 0}, {0, 1}}),
                Alpha = 0,
                Beta = 0,
                K = DenseMatrix.OfArray(new double[,] {{6, -2}, {-2, 4}}),
                R = new LoadsVector(new[] {"0.0", "10.0"}),
                MovementU = DenseVector.OfArray(new double[] {0, 0}),
                SpeedU = DenseVector.OfArray(new double[] {0, 0}),
                T0 = 0,
                DeltaT = 0.28,
                Tk = 5
            };

            var method = new CentralDifference();

            var result = method.Solve(inputs);

            Assert.AreEqual(0, result[1].MovementU[0]);
            Assert.IsTrue(Math.Abs(0.392 - result[1].MovementU[1]) < 0.01);

            Assert.IsTrue(Math.Abs(0.0307 - result[2].MovementU[0]) < 0.01);
            Assert.IsTrue(Math.Abs(1.45 - result[2].MovementU[1]) < 0.01);

            Assert.IsTrue(Math.Abs(1.02 - result[12].MovementU[0]) < 0.01);
            Assert.IsTrue(Math.Abs(2.60 - result[12].MovementU[1]) < 0.01);
        }
    }
}
