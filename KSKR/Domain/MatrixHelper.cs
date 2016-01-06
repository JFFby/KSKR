using System;
using System.Linq;
using Domain.Common;
using Domain.Enums;
using MathNet.Numerics.LinearAlgebra;

namespace Domain
{
    public sealed class MatrixHelper
    {
        private string vectorString;
        private VectorType type;
        private string[][] matrix;
        private readonly int columns, rows;
        private int k, m;
        private string error = null;
        private bool isLinearMatrix;

        public MatrixHelper(string vector, VectorType type, int columns, int rows, bool isLinearMatrix)
        {
            this.type = type;
            vectorString = vector;
            this.columns = columns;
            this.rows = rows;
            matrix = !isLinearMatrix ? SplitVector() : SplitLinerMatrix();
            this.isLinearMatrix = isLinearMatrix;
        }

        public static Matrix<double> LinearToNormal(Matrix<double> linear, int width)
        {
            var b = 1 + width * 2;
            return Convert(linear.RowCount, b, width, (i, j) => linear[i, j]);
        }


        private static Matrix<double> Convert(int size, int b, int f, Func<int, int, double> getMatrixItem)
        {
            var matr = Matrix<double>.Build.Dense(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int y = j - i + f;
                    matr[i, j] = y >= 0 && y < b ? getMatrixItem(i, y) : 0;
                }
            }

            return matr;
        }

        public Tuple<object, string> Resolve()
        {
            object obj = null;
            switch (type)
            {
                case VectorType.Matrix:
                    obj = this.isLinearMatrix ? CreateLinearMatrix() : CreateMatrix();
                    break;
                case VectorType.Vector:
                    obj = СreateVector();
                    break;
                case VectorType.LoadsVector:
                    obj = CreateLoadsVector();
                    break;
            }

            return new Tuple<object, string>(obj, error);
        }

        private LoadsVector CreateLoadsVector()
        {
            var vector = new LoadsVector(matrix.SelectMany(x => x).ToArray());
            return vector;
        }

        private Vector<double> СreateVector()
        {
            var vector = Vector<double>.Build.Dense(columns, 0f);
            for (int i = 0; i < columns; i++)
            {
                vector[i] = TryGetMatrixValue(i, 0);
            }

            return vector;
        }

        private Matrix<double> CreateLinearMatrix()
        {
            var b = k + m + 1;
            return Convert(this.matrix.Length, b, k, TryGetMatrixValue);
        }

        private Matrix<double> CreateMatrix()
        {
            var matr = Matrix<double>.Build.Dense(rows, columns);
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    matr[i, j] = TryGetMatrixValue(i, j);
                }
            }

            return matr;
        }

        private double TryGetMatrixValue(int i, int j)
        {
            try
            {
                return double.Parse(matrix[i][j]);
            }
            catch (Exception e)
            {
                SetParseError(i, j);
                return 0;
            }
        }

        private string[][] SplitLinerMatrix()
        {
            var stringRows = vectorString.Split('\n');
            m = k = Int32.Parse(stringRows[0].Trim());
            var matrixRows = new string[stringRows.Length - 1];
            Array.Copy(stringRows, 1, matrixRows, 0, stringRows.Length - 1);

            return BuildVector(matrixRows);
        }

        private string[][] SplitVector()
        {
            var stringRows = vectorString.Split('\n');
            RemoveColWidth(ref stringRows);
            CheckVectorSize(stringRows.Length == columns);

            return BuildVector(stringRows);
        }

        private string[][] BuildVector(string[] stringRows)
        {
            var vector = new string[stringRows.Length][];
            for (int i = 0; i < stringRows.Length; i++)
            {
                var row = stringRows[i].Trim();
                vector[i] = row.Split(' ');
            }

            return vector;
        }

        private void RemoveColWidth(ref string[] stringRows)
        {
            if (stringRows[0].Trim().Split(' ').Length == 1 && stringRows[1].Trim().Split(' ').Length > 1) 
            {
                var matrixRows = new string[stringRows.Length - 1];
                Array.Copy(stringRows, 1, matrixRows, 0, stringRows.Length - 1);
                stringRows = matrixRows;
            }
        }

        private void SetParseError(int i, int j)
        {
            if (string.IsNullOrEmpty(error))
            {
                error = string.Format("Не удалось считать значение {0};{1}", i, j);
            }
        }

        private void CheckVectorSize(bool eqals)
        {
            if (!eqals && string.IsNullOrEmpty(error))
            {
                var vectorType = type == VectorType.Matrix ? "считываемой матрицы" : "считываемого вектора";
                var message = string.Format("Внимание! Размерность {0} не совпалает с текущей размерностью." +
                                            " Значения могуть быть не корректны.", vectorType);
                error = message;
            }
        }
    }
}
