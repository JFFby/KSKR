using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Numark
{
    public class Numark : IMethod
    {
        private Inputs Inputs;
        private double alpha = 0.25;
        private double delta = 0.5;

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
            if(Inputs.Delta==0)
            {
                Inputs.Delta = delta;
            }
            alpha = 0.25 * Math.Pow((0.5 + Inputs.Delta),2);
            var dt = Inputs.DeltaT;
            var a0 = 1 / (alpha * Math.Pow(dt, 2));
            var a1 = Inputs.Delta / (alpha * dt);
            var a2 = 1 / (alpha * dt);
            var a3 = (1 / (2 * alpha)) - 1;
            var a4 = (Inputs.Delta / alpha) - 1;
            var a5 = (dt / 2) * ((Inputs.Delta / alpha) - 2);
            var a6 = dt * (1 - Inputs.Delta);
            var a7 = Inputs.Delta * dt;
            return new[] {a0, a1, a2, a3, a4, a5, a6, a7 };
        }

        private IList<State> Solve(State state)
        {
            var states = new List<State>();
            var ic = IntegrationConstants();
            var pastU = state.MovementU;
            var effectiveK = Inputs.K + ic[0] * Inputs.M + ic[1] * Inputs.C;
            states.Add(state);
            for (double t = Inputs.T0; t < Inputs.Tk; t += Inputs.DeltaT)
            {
                var lastState = states.Last();
                var effectiveR = Inputs.R.ToVector(t + Inputs.DeltaT) +
                                    Inputs.M * (ic[0] * lastState.MovementU + ic[2] * lastState.SpeedU +
                                              ic[3] * lastState.AccelerationU) +
                                    Inputs.C * (ic[1] * lastState.MovementU
                                              + ic[4] * lastState.SpeedU + ic[5] * lastState.AccelerationU);
                var nextMovementU = effectiveR * effectiveK.Inverse();

                var nextAcceleration = ic[0] * (nextMovementU - lastState.MovementU) - ic[2] * lastState.SpeedU -
                                ic[3] * lastState.AccelerationU;

                var nextSpeed = lastState.SpeedU + ic[6] * lastState.AccelerationU +
                                ic[7] * nextAcceleration;

                states.Add(new State(t, effectiveR, nextMovementU, nextSpeed, nextAcceleration,
                    lastState.NextStateMovementU));
                
            }
            return states;
        }

    }
}
