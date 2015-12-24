using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public sealed class MatrixTransform
    {
        [TestMethod]
        public void Test1()
        {
            const int N = 5;
            const int KL = 2, KU = 1, B = KL + KU + 1;

            var a = new int[N, N]
            {
                {11, 12, 00, 00, 00},
                {21, 22, 23, 00, 00},
                {31, 32, 33, 34, 00},
                {00, 42, 43, 44, 45},
                {00, 00, 53, 54, 55}
            };

            var expected = new int[N, B]
            {
                {00, 00, 11, 12},
                {00, 21, 22, 23},
                {31, 32, 33, 34},
                {42, 43, 44, 45},
                {53, 54, 55, 00}
            };

            ProcessTest(N, KL, KU, a, expected);
        }

        [TestMethod]
        public void Test2()
        {
            const int N = 6;
            const int KL = 1, KU = 1, B = KL + KU + 1;

            var a = new int[N, N]
            {
                {11, 12, 00, 00, 00, 00},
                {21, 22, 23, 00, 00, 00},
                {00, 32, 33, 34, 00, 00},
                {00, 00, 43, 44, 45, 00},
                {00, 00, 00, 54, 55, 56},
                {00, 00, 00, 00, 65, 66}
            };

            var expected = new int[N, B]
            {
                {00, 11, 12},
                {21, 22, 23},
                {32, 33, 34},
                {43, 44, 45},
                {54, 55, 56},
                {65, 66, 0}
            };

            ProcessTest(N, KL, KU, a, expected);
        }

        [TestMethod]
        public void Test3()
        {
            const int N = 5;
            const int KL = 1, KU = 2, B = KL + KU + 1;

            var a = new int[N, N]
            {
                {11, 12, 13, 00, 00},
                {21, 22, 23, 24, 00},
                {00, 32, 33, 34, 35},
                {00, 00, 43, 44, 45},
                {00, 00, 00, 54, 55}
            };

            var expected = new int[N, B]
            {
                {00, 11, 12, 13},
                {21, 22, 23, 24},
                {32, 33, 34, 35},
                {43, 44, 45, 00},
                {54, 55, 00, 00}
            };

            ProcessTest(N, KL, KU, a, expected);
        }

        private void ProcessTest(int n, int kl, int ku, int[,] a, int[,] expected)
        {
            var B = 1 + kl + ku;

            var b = new int[n, B];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < B; j++)
                {
                    int y = i + j - kl;
                    b[i, j] = y >= 0 && y < n ? a[i, y] : 0;
                    Assert.AreEqual(b[i, j], expected[i, j]);
                }
            }

            var c = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int y = j - i + kl;
                    c[i, j] = y >= 0 && y < B ? b[i, y] : 0;
                    Assert.AreEqual(a[i, j], c[i, j]);
                }
            }
        }
    }
}
