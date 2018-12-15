using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System;

namespace Dice.States
{
    internal class DevideState<T> : State
    {
        private readonly DevideTable<T> table;
        public T Value { get; }

        internal DevideState(State parent, P<T> p, T value) : base(parent)
        {
            this.table = new DevideTable<T>(this, p, value);
            this.Value = value;
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            return base.GetStatePropability(manager) * this.table.GetPartPropability(manager);
        }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> index, in WhileManager manager)
        {
            if (this.table.Contains(index, manager))
                return (manager, this.table);
            return base.GetTable(index, manager);
        }

        public override void PrepareOptimize(IEnumerable<IP> p, in WhileManager manager)
        {
            base.PrepareOptimize(p, manager);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.ConditionVariable });
        }
    }

}
