using System;
using MathNet.Numerics.LinearAlgebra;

namespace Domain
{
    public class LinerMatrixCreationEventArgs : EventArgs
    {
        public LinerMatrixCreationEventArgs(Matrix<double> matrix)
        {
            Matrix = matrix;
        }

        public Matrix<double> Matrix { get; private set; } 
    }
}
