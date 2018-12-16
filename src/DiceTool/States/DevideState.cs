using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System;
using Dice.Caches;

namespace Dice.States
{
    internal class DevideState<T> : TableState<DevideTable<T>>
    {
        public T Value { get; }

        public override DevideTable<T> Table => table;

        private readonly P<T> condition;
        private readonly DevideTable<T> table;

        internal DevideState(State parent, P<T> condition, T value) : base(parent)
        {
            this.condition = condition;
            this.table = new DevideTable<T>(this, condition, value);
            this.Value = value;
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            var totoalPropability = this.table.GetPartPropability(manager);

            return base.GetStatePropability(manager) * totoalPropability;
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.condition });
        }
    }

}
