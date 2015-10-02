using Domain.Habolt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Helpers;

namespace Tests
{
    [TestClass]
    public class HaboltTest
    {
        [TestMethod]
        //Довольо большие отклонения от эталонных значений
        public void HabotltTest1()
        {
            var inputs = TestHelper.GetTestInputData();

            var result = new Habolt().Solve(inputs);

            TestHelper.Assert(2.11, result[7].MovementU[0], 0.4);
            TestHelper.Assert(5.31, result[7].MovementU[1],0.105);

           TestHelper.Assert(1.72, result[12].MovementU[0]);
           TestHelper.Assert(2.28, result[12].MovementU[1], 0.07);

        }
    }
}
