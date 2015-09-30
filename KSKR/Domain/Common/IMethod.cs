﻿using System.Collections.Generic;

namespace Domain.Common
{
    public interface IMethod
    {
        IList<State> Solve(Inputs initialState);
    }
}
