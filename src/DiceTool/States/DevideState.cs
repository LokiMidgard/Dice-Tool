using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System;
using Dice.Caches;
using System.Threading;

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

        public override double GetStatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();


            return this.cache.GetOrCreate(nameof(GetStatePropability), manager, this.CalculatePropability, cancellation);
        }

        private double CalculatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var totoalPropability = this.table.GetPartPropability(manager, cancellation);

            return base.GetStatePropability(manager, cancellation) * totoalPropability;
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetOptimizedVariablesForParent(cancellation).Concat(new IP[] { this.condition });
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            if (this.GetStatePropability(manager, cancellation) == 0.0)
            {
                return (manager, this.emptyTable);
            }
            return base.GetTable(variable, manager, cancellation);
        }

    }

}
