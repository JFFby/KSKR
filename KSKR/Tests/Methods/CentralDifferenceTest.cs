using System;
using Domain.CentralDifference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests
{
    [TestClass]
    public class CentralDifferenceTest
    {
        [TestMethod]
        public void Test1()
        {
            var inputs = TestHelper.GetTestInputData();
            var method = new CentralDifference();

            var result = method.Solve(inputs);

            Assert.AreEqual(0, result[1].MovementU[0]);
            TestHelper.Assert(0.392, result[1].MovementU[1]);

            TestHelper.Assert(0.0307, result[2].MovementU[0]);
            TestHelper.Assert(1.45, result[2].MovementU[1]);
            
            TestHelper.Assert(1.02, result[12].MovementU[0]);
            TestHelper.Assert(2.60, result[12].MovementU[1]);
        }
    }
}
