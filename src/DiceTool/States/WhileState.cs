using System;
using System.Collections.Generic;
using System.Text;

namespace Dice.States
{
    internal class WhileState : State
    {
        private readonly (IP issuedVariable, IP originalVariable, IP whileBodyVariable)[] p;

        public DevideState<bool> EndState { get; }

        public WhileState(State parent, P<bool> condition) : base(parent)
        {
            this.Condition = condition;
            this.p = p;
            EndState = new DevideState<bool>(this, condition, false);
            ContinueState = new DevideState<bool>(this, condition, true);

        }

        public ReadOnlySpan<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)> VariableMapping => new ReadOnlySpan<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)>(this.p);

        public P<bool> Condition { get; }
        public DevideState<bool> ContinueState { get; }
    }
}
