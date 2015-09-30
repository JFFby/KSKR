using MathNet.Numerics.LinearAlgebra;

namespace Domain.Common
{
    public class State
    {
        public State(
            double time, 
            Vector<double> r,
            Vector<double> movementU,
            Vector<double> speedU,
            Vector<double> accelerationU,
            Vector<double> nextStateMovementU)
        {
            NextStateMovementU = nextStateMovementU;
            AccelerationU = accelerationU;
            SpeedU = speedU;
            Time = time;
            MovementU = movementU;
            R = r;
        }

        public double Time { get; private set; }

        public Vector<double> R { get; private set; }

        public Vector<double> MovementU { get; private set; }

        public Vector<double> SpeedU { get; private set; }

        public Vector<double> AccelerationU { get; private set; }

        public Vector<double> NextStateMovementU { get; private set; } 
    }
}
