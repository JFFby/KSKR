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
            var size = this.matrix.Length;
            var b = k + m + 1;
            var matr = Matrix<double>.Build.Dense(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int y = j - i + k;
                    matr[i, j] = y >= 0 && y < b ? TryGetMatrixValue(i, y) : 0;
                }
            }

            return matr;
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
            m = Int32.Parse(stringRows[0].Trim());
            k = Int32.Parse(stringRows[1].Trim());
            var matrixRows = new string[stringRows.Length - 2];
            Array.Copy(stringRows, 2, matrixRows, 0, stringRows.Length - 2);

            return BuildVector(matrixRows);
        }

        private string[][] SplitVector()
        {
            var stringRows = vectorString.Split('\n');
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
