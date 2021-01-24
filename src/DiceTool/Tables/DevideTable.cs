using Dice.States;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dice.Tables
{
    internal class DevideTable<T> : Table
    {
        private readonly T value;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();
        private readonly State state;
        private readonly P<T> conditionVariable;

        internal (WhileManager manager, Table table) GetOriginal(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return this.state.Parent.GetTable(this.conditionVariable, manager, cancellation);
        }

        public DevideTable(State state, P<T> p, T value) : base(state)
        {
            this.state = state;
            this.conditionVariable = p;
            this.value = value;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return this.GetOriginal(manager, cancellation).GetVariables(cancellation);
        }

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            return this.GetIndexLookup(manager, cancellation).Length;
        }

        public double GetPartPropability(in WhileManager manager, CancellationToken cancellation)
        {
            return this.cache.GetOrCreate(nameof(GetPartPropability), manager, this.CalculatePropability, cancellation);
        }

        private double CalculatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var partPropability = 0.0;
            for (var i = 0; i < this.GetCount(manager, cancellation); i++)
                partPropability += this.GetOriginal(manager, cancellation).GetValue(PropabilityKey, this.GetIndexLookup(manager, cancellation)[i], cancellation);

            if (partPropability == 0)
                this.cache.Update(nameof(GetIndexLookup), manager, new int[0]);
            return partPropability;
        }

        private int[] GetIndexLookup(in WhileManager manager, CancellationToken cancellation)
        {
            return this.cache.GetOrCreate(nameof(GetIndexLookup), manager, this.CalculateIndexLookup, cancellation);
        }

        private int[] CalculateIndexLookup(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var list = new List<int>();

            var table = this.GetOriginal(manager, cancellation);
            var rowCount = table.GetCount(cancellation);
            for (var i = 0; i < rowCount; i++)
            {
                cancellation.ThrowIfCancellationRequested();


                if (Equals(table.GetValue(this.conditionVariable, i, cancellation), this.value))
                    list.Add(i);
            }
            var indexLookup = list.ToArray();
            return indexLookup;
        }

        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            if (p.Id == PropabilityKey.Id)
                return this.GetOriginal(manager, cancellation).GetValue(PropabilityKey, this.GetIndexLookup(manager, cancellation)[index], cancellation) / this.GetPartPropability(manager, cancellation); // Part propability will taken into account later.

            return this.GetOriginal(manager, cancellation).GetValue(p, this.GetIndexLookup(manager, cancellation)[index], cancellation);
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return this.GetOriginal(manager, cancellation).Contains(key, cancellation);
        }
    }

}
