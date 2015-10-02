using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class NumericsTest
    {
        [TestMethod]
        public void Matr_Vector_Multiply()
        {
            Matrix<double> M = DenseMatrix.OfArray(new double[,]
            {
                {2, 0}, {0, 1}
            });
            Vector<double> U = DenseVector.OfArray(new double[] { 0, 10 });

            var result = (M * U).ToArray();
            var expected = new[] { 0, 10.0 };

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.AreEqual(expected[i], result[i]);
            }
        }

        [TestMethod]
        public void Matrix_Matrix_Sum()
        {
            var M = DenseMatrix.OfRows(new List<List<double>> { new List<double> { 2, 0 }, new List<double> { 0, 1 } });
            var zero = DenseMatrix.Zero;
            var C = DenseMatrix.Create(2, 2, zero);

            var result = 12.8 * M + 1.79 * C;
            var expected = DenseMatrix.OfArray(new double[,]
            {
                {25.6, 0}, {0, 12.8}
            });

            for (int i = 0; i < result.ColumnCount; i++)
            {
                for (int j = 0; j < result.RowCount; j++)
                {
                    Assert.AreEqual(expected.Row(i)[j], result.Row(i)[j]);
                }
            }
        }


        [TestMethod]
        public void Transpose()
        {
            Matrix<double> M = DenseMatrix.OfArray(new double[,]
            {
                {1, 2}, {3, 4}
            });

            var result = M.Transpose();

            for (int i = 0; i < M.ColumnCount; i++)
            {
                for (int j = 0; j < M.RowCount; j++)
                {
                    Assert.AreEqual(result.Row(j)[i], M.Row(i)[j]);
                }
            }
        }

        [TestMethod]
        public void MatrixDivision()
        {
            Matrix<double> M = DenseMatrix.OfArray(new double[,]
            {
                {2, 0}, {0, 1}
            });

            Vector<double> U = DenseVector.OfArray(new double[] { 0, 10 });

            var result = U * M.Inverse();
            Vector<double> expected = DenseVector.OfArray(new double[] { 0, 10 });

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expected[i], result[i]);
            }
        }
    }
}