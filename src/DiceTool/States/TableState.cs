﻿using Dice.Tables;

namespace Dice.States
{
    internal abstract class TableState<T> : State
        where T : Table
    {

        public abstract T Table { get; }

        public TableState(State parent) : base(parent)
        {
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            if (this.Table.Contains(variable, manager))
                return (manager, this.Table);
            return base.GetTable(variable, manager);
        }

        public override bool Contains(IP variable, in WhileManager manager)
        {
            if (this.Table.Contains(variable, manager))
                return true;
            return base.Contains(variable, manager);
        }

    }

}
