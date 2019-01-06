using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewConstState<T> : TableState<ConstTable<T>>
    {
        private readonly ConstTable<T> table;
        private readonly P<T> variable;

        public override ConstTable<T> Table => table;

        public NewConstState(State parent, P<T> variable, T value) : base(parent)
        {
            this.table = new ConstTable<T>(this, variable, value);
            this.variable = variable;
        }

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState()
        {
            return base.GetVarialesProvidedByThisState().Concat(new IP[] { this.variable });
        }


    }

}
