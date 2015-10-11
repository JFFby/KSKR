using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Numark
{
    public class Numark
    {
        private Inputs Inputs;
        private const double alpha = 0.5;
        private double delta = 0;

        private double[] IntegrationConstants()
        {
            delta = 0.25 * Math.Pow((0.5 * alpha), 2);
            var dt = Inputs.DeltaT;
            var a0 = 1 / alpha * Math.Pow(dt, 2);
            var a1 = delta / alpha * dt;
            var a2 = 1 / alpha * dt;
            var a3 = (1 / (2 * alpha)) - 1;
            var a4 = (delta / alpha) - 1;
            var a5 = (dt / 2) * ((delta / alpha) - 2);
            var a6 = dt * (1 - delta);
            var a7 = delta * dt;
            return new[] { a1, a2, a3, a4, a5, a6, a7 };
        }

        private IList<State> Solve(IList<State> states)
        {
            var ic = IntegrationConstants();
            var effectiveK = Inputs.K + ic[0] * Inputs.M + ic[1] * Inputs.C;
            for (double t = Inputs.T0; t < Inputs.Tk; t += Inputs.DeltaT)
            {
                var ancientState = states.ElementAt(states.Count - 2);
                var lastState = states.Last();
                var effectiveR = Inputs.R.ToVector(t + Inputs.DeltaT) +
                                    Inputs.M * (ic[0] * lastState.NextStateMovementU + ic[2] * lastState.NextStateMovementU +
                                              ic[3] * lastState.NextStateMovementU) +
                                    Inputs.C * (ic[1] * lastState.NextStateMovementU
                                              + ic[4] * lastState.NextStateMovementU + ic[5] * ancientState.NextStateMovementU);
                var nextMovementU = effectiveR * effectiveK.Inverse();

                

                var nextAcceleration = ic[0] *(nextMovementU - lastState.NextStateMovementU) - ic[2] * lastState.NextStateMovementU -
                                ic[3] * lastState.NextStateMovementU;

                var nextSpeed = nextMovementU + ic[6] * lastState.NextStateMovementU +
                                ic[7] * nextAcceleration;

                states.Add(new State(t, effectiveR, lastState.NextStateMovementU, nextSpeed, nextAcceleration,
                    nextMovementU));
            }
            return states;
        }

    }
}
