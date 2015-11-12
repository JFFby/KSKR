using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;
using Domain.Vilson;
namespace Tests.Methods
{
    [TestClass]
    public class VilsonTest
    {
        [TestMethod]
        public void VilsenResulTest()
        {
            var inputs = TestHelper.GetTestInputData();
            Vilson method = new Vilson();

            var result = method.Solve(inputs);

            TestHelper.Assert(0.00605, result[1].MovementU[0]);
            TestHelper.Assert(0.366, result[1].MovementU[1]);

            TestHelper.Assert(0.952, result[5].MovementU[0]);
            TestHelper.Assert(4.88, result[5].MovementU[1]);

        }

    }
}
