using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vilsen
{
    public class Vilsen : IMethod
    {
        private const double teta = 1.4;
        private Inputs Inputs;

        public IList<State> Solve(Inputs initialState)
        {
            Inputs = initialState;
            var state = SolveInitialState();
            return Solve(state);
        }
        private State SolveInitialState()
        {
            var rVector = Inputs.R.ToVector(Inputs.T0);
            var u__ = (rVector - Inputs.C * Inputs.SpeedU - Inputs.K * Inputs.MovementU) * Inputs.M.Inverse();
            return new State(Inputs.T0, rVector, Inputs.MovementU, Inputs.SpeedU, u__, null);
        }

        private double[] IntegrationConstants()
        {
            var dt = Inputs.DeltaT;
            var a0 = 6 / Math.Pow(teta * dt, 2);
            var a1 = 3 / teta * dt;
            var a2 = 2 * a1;
            var a3 = teta * dt / 2;
            var a4 = a0 / teta;
            var a5 = -a2 / teta;
            var a6 = 1 - (3 / teta);
            var a7 = dt / 2;
            var a8 = Math.Pow(dt, 2) / 6;
            return new[] {a0, a1, a2, a3, a4, a5, a6, a7, a8 };
        }

        private IList<State> Solve(State state)
        {
            var states = new List<State>();
            var dt = Inputs.DeltaT;
            var ic = IntegrationConstants();
            var effectiveK = Inputs.K + ic[0] * Inputs.M + ic[1] * Inputs.C;
            //t+teta*dt ------ проверить
            for (double t = Inputs.T0; t < Inputs.Tk; t += Inputs.DeltaT*teta)
            {
                var lastState = states.Last();
                var effectiveR = Inputs.R.ToVector(t) +
                    teta * (Inputs.R.ToVector(t + Inputs.DeltaT) - Inputs.R.ToVector(t)) +
                    Inputs.M * (ic[0] * lastState.NextStateMovementU + ic[2] * lastState.NextStateMovementU +
                                              2 * lastState.NextStateMovementU) +
                    Inputs.C * (ic[1] * lastState.NextStateMovementU
                                              + 2 * lastState.NextStateMovementU + ic[3] * lastState.NextStateMovementU);

                var nextMovementU = effectiveR * effectiveK.Inverse();

                var nextAcceleration = ic[4] * (nextMovementU - lastState.NextStateMovementU) + ic[5] * lastState.NextStateMovementU + ic[6] * lastState.NextStateMovementU;

                var nextSpeed = lastState.NextStateMovementU + ic[7] * (nextAcceleration + lastState.NextStateMovementU);

                var nextMove = lastState.NextStateMovementU + dt * (lastState.NextStateMovementU) + ic[8] * (nextAcceleration + 2 * lastState.NextStateMovementU);

                states.Add(new State(t, effectiveR, lastState.NextStateMovementU, nextSpeed, nextAcceleration,
                   nextMovementU));
            }
            return states;
        }
    }
}
