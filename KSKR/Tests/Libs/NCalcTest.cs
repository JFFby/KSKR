using System;
using Domain.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCalc;
using Expression = NCalc.Expression;

namespace Tests
{
    [TestClass]
    public class NCalcTest
    {
        [TestMethod]
        public void Test1()
        {
            var expr = new Expression("cos([t])", EvaluateOptions.IgnoreCase);

            expr.Parameters["t"] = 0;
            var result = expr.Evaluate();

            Assert.AreEqual(1d, result);

            expr.Parameters["t"] = Math.PI;
            result = expr.Evaluate();

            Assert.AreEqual(-1d, result);
        }

        [TestMethod]
        public void SimpeValue()
        {
            var expr = new Expression("1.0");
            var result = (double)expr.Evaluate();
            Assert.AreEqual(1d, result);
        }

        [TestMethod]
        public void SimpeValue2()
        {
            var vactor = new string[] {"1 + 2.0 - 0.3"};
            var expr = new LoadsVector(vactor);
            var result = expr.ToVector();
            Assert.AreEqual(2.7d, result[0]);
        }

        [TestMethod]
        public void Square()
        {
            var expr = new Expression("[t]*[t]");
            expr.Parameters["t"] = 2;
            var result = expr.Evaluate();
            Assert.AreEqual(4, result);
        }
    }
}
