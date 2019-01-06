using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewVariableState<T> : TableState<SingelVariableTable<T>>
    {

        private readonly SingelVariableTable<T> table;
        private readonly P<T> variable;

        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            table = new SingelVariableTable<T>(this, variable, distribution);
            this.variable = variable;
        }

        public override SingelVariableTable<T> Table => table;

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState()
        {
            return base.GetVarialesProvidedByThisState().Concat(new IP[] { this.variable });
        }

    }

}
