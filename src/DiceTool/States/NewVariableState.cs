using Dice.Tables;
using System;

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



        public override (WhileManager manager, Table table) GetTable<T1>(P<T1> index, in WhileManager manager)
        {
            if (index.Id == this.Id)
                return (manager, this.table);
            return base.GetTable(index, manager);
        }
    }

}
