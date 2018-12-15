using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewVariableState<T> : State
    {

        private readonly SingelVariableTable<T> table;
        private string Id { get; }

        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new SingelVariableTable<T>(variable, distribution);
        }



        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        {
            if (variable.Id == this.Id)
                return (manager, this.table);
            return base.GetTable(variable, index, manager);
        }
    }

}
