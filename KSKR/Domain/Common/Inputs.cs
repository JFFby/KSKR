using MathNet.Numerics.LinearAlgebra;

namespace Domain.Common
{
    public class Inputs
    {
        public Matrix<double> M { get; set; }

        public Matrix<double> K { get; set; }

        public double T0 { get; set; }

        public double Tk { get; set; }

        public double DeltaT { get; set; }

        public double Alpha { get; set; }

        public double Beta { get; set; }

        public Matrix<double> C
        {
            get { return Alpha * K + Beta * M; }
        }

        public Vector<double> MovementU { get; set; }

        public Vector<double> SpeedU { get; set; }
        public LoadsVector R { get; set; }
    }
}
