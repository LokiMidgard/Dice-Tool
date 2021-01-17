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

        public override DevideTable<T> Table => this.table;

        private readonly P<T> condition;
        private readonly DevideTable<T> table;
        private readonly EmptyTable emptyTable;

        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        internal DevideState(State parent, P<T> condition, T value) : base(parent)
        {
            this.condition = condition;
            this.table = new DevideTable<T>(this, condition, value);
            this.emptyTable = new EmptyTable(this);
            this.Value = value;
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            return this.cache.GetOrCreate(nameof(GetStatePropability), manager, this.CalculatePropability);
        }

        private double CalculatePropability(in WhileManager manager)
        {
            var totoalPropability = this.table.GetPartPropability(manager);

            return base.GetStatePropability(manager) * totoalPropability;
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.condition });
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            if (this.GetStatePropability(manager) == 0.0)
            {
                return (manager, this.emptyTable);
            }
            return base.GetTable(variable, manager);
        }

    }

}
