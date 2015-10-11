using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;
using Domain.Vilsen;
namespace Tests.Methods
{
    [TestClass]
    public class VilsonTest
    {
        [TestMethod]
        public void VilsenResulTest()
        {
            var inputs = TestHelper.GetTestInputData();
            Vilsen method = new Vilsen();

            var result = method.Solve(inputs);

            Assert.AreEqual(0, result[1].MovementU[0]);
            TestHelper.Assert(0.366, result[1].MovementU[1]);

        }

    }
}
