using System;
using System.Collections.Generic;
using System.Text;

namespace Dice.States
{
    internal class WhileState : State
    {
        private readonly (IP issuedVariable, IP originalVariable, IP whileBodyVariable)[] p;

        public DevideState EndState { get; }

        public WhileState(State parent, P<bool> condition, (IP issuedVariable, IP originalVariable, IP whileBodyVariable)[] p) : base(parent)
        {
            this.Condition = condition;
            this.p = p;
            EndState = new DevideState(this, condition, false);
            ContinueState = new DevideState(this, condition, true);

        }

        public ReadOnlySpan<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)> VariableMapping => new ReadOnlySpan<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)>(this.p);

        public P<bool> Condition { get; }
        public DevideState ContinueState { get; }
    }
}
