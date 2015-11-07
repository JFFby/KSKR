using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using MathNet.Numerics.LinearAlgebra;

namespace Domain.CentralDifference
{
    public class CentralDifference : IMethod
    {
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

        private double[] PrepareIntegrationConstants()
        {
            var a0 = 1 / (Math.Pow(Inputs.DeltaT, 2));
            var a1 = 1 / (2 * Inputs.DeltaT);
            var a2 = 2 * a0;
            return new double[] { a0, a1, a2, 1 / a2 };
        }

        private IList<State> Solve(State initState)
        {
            var ic = PrepareIntegrationConstants();
            var states = new List<State>();
            var pastU = initState.MovementU - Inputs.DeltaT * initState.SpeedU + ic[3] * initState.AccelerationU;
            var effectiveM = ic[0] * Inputs.M + ic[1] * Inputs.C;
            states.Add(AdjustInitialState(initState, ic[2], ic[0], ic[1], pastU, effectiveM));
            for (double t = Inputs.T0 + Inputs.DeltaT; t < Inputs.Tk; t += Inputs.DeltaT)
            {
                var previosState = states.Last();
                var effectiveR = Inputs.R.ToVector(t) - (Inputs.K - ic[2] * Inputs.M) * previosState.NextStateMovementU -
                    (ic[0] * Inputs.M - ic[1] * Inputs.C) * previosState.MovementU;
                var nextStateMovementU = effectiveR * effectiveM.Inverse();
                var speedU = ic[1] * (nextStateMovementU - previosState.MovementU);
                var accelerationU = ic[0] * (previosState.MovementU - 2 * previosState.NextStateMovementU + nextStateMovementU);
                states.Add(new State(t, effectiveR, previosState.NextStateMovementU, speedU, accelerationU, nextStateMovementU));
            }
            return states;
        }

        private State AdjustInitialState(State state, double a2, double a0, double a1, Vector<double> pastU, Matrix<double> effectiveM)
        {
            var effectiveR = Inputs.R.ToVector(Inputs.T0) - (Inputs.K - a2 * Inputs.M) * Inputs.MovementU - (a0 * Inputs.M - a1 * Inputs.C) * pastU;
            var nextStateMovementU = effectiveR * effectiveM.Inverse();

            return new State(Inputs.T0, effectiveR, state.MovementU, state.SpeedU, state.AccelerationU, nextStateMovementU);
        }
    }
}
