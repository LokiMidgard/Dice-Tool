using Dice.Tables;
using System;

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


        public override (WhileManager manager, Table table) GetTable<T1>(P<T1> index, in WhileManager manager)
        {
            if (index.Id == this.Id)
                return (manager, this.table);
            return base.GetTable(index, manager);
        }
    }

}
