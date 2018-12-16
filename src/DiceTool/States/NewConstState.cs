using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewConstState<T> : TableState<ConstTable<T>>
    {
        private readonly ConstTable<T> table;

        public override ConstTable<T> Table => table;

        public NewConstState(State parent, P<T> variable, T value) : base(parent)
        {
            this.table = new ConstTable<T>(variable, value);
        }

    }

}
