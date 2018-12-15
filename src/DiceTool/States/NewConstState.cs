using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dice.States
{
    internal class NewConstState<T> : State
    {
        private readonly ConstTable<T> table;
        private string Id { get; }

        public NewConstState(State parent, P<T> variable, T value) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new ConstTable<T>(variable, value);
        }


        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        {
            if (variable.Id == this.Id)
                return (manager, this.table);
            return base.GetTable(variable, index, manager);
        }
    }

}
