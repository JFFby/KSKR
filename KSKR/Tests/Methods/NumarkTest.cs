using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;
using Domain.Numark;

namespace Tests.Methods
{
    [TestClass]
    public class NumarkTest
    {
        [TestMethod]
        public void NumarkTestMethod()
        {
            var inputs = TestHelper.GetTestInputData();
            var method = new Numark();

            var result = method.Solve(inputs);

            TestHelper.Assert(0.00673, result[1].MovementU[0]);
            TestHelper.Assert(0.364, result[1].MovementU[1]);

            TestHelper.Assert(2.76, result[8].MovementU[0]);
            TestHelper.Assert(4.48, result[8].MovementU[1]);
        }
    }
}
