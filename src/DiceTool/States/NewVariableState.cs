using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewVariableState<T> : TableState<SingelVariableTable<T>>
    {

        private readonly SingelVariableTable<T> table;


        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            table = new SingelVariableTable<T>(variable, distribution);
        }

        public override SingelVariableTable<T> Table => table;
    }

}
