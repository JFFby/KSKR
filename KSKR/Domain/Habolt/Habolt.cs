using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;

namespace Domain.Habolt
{
    public class Habolt : IMethod
    {
        private Inputs Inputs;

        public IList<State> Solve(Inputs initialState)
        {
            Inputs = initialState;
            var states = GetFistSteps();
            return Solve(states);
        }

        private IList<State> GetFistSteps()
        {
            var dt = Inputs.DeltaT * 3;
            var oldTk = Inputs.Tk;
            Inputs.Tk = Inputs.DeltaT * 3;
            var states = new CentralDifference.CentralDifference().Solve(Inputs);
            Inputs.T0 = dt + Inputs.DeltaT;
            Inputs.Tk = oldTk;
            return states;
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
                                    Inputs.M * (ic[2] * lastState.NextStateMovementU + ic[4] * lastState.MovementU +
                                              ic[6] * ancientState.MovementU) +
                                    Inputs.C * (ic[3] * lastState.NextStateMovementU
                                              + ic[5] * lastState.MovementU + ic[1] * ancientState.MovementU);
                var nextMovementU = effectiveR * effectiveK.Inverse();
                var nextSpeed = ic[1] * nextMovementU - ic[3] * lastState.NextStateMovementU -
                                ic[5] * lastState.MovementU - ic[7] * ancientState.MovementU;
                var nextAcceleration = ic[0] * nextMovementU - ic[2] * lastState.NextStateMovementU -
                                ic[4] * lastState.MovementU - ic[6] * ancientState.MovementU;

                states.Add(new State(t, effectiveR, lastState.NextStateMovementU, nextSpeed, nextAcceleration,
                    nextMovementU));
            }
            return states;
        }

        private double[] IntegrationConstants()
        {
            var dt = Inputs.DeltaT;
            var a0 = 2 / Math.Pow(dt, 2);
            var a3 = 3 / dt;
            return new[] { a0, 11 / (6 * dt), 5 / Math.Pow(dt, 2), a3, -2 * a0, -a3 / 2, a0 / 2, a3 / 9 };
        }
    }
}
