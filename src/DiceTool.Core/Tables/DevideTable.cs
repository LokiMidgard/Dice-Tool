using Dice.States;
using System;
using System.Collections.Generic;

namespace Dice.Tables
{
    internal class DevideTable<T> : Table
    {
        private readonly T value;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();
        private readonly State state;
        private readonly P<T> conditionVariable;

        internal (WhileManager manager, Table table) GetOriginal(in WhileManager manager) => this.state.Parent.GetTable(this.conditionVariable, manager);

        public DevideTable(State state, P<T> p, T value) : base(state)
        {
            this.state = state;
            this.conditionVariable = p;
            this.value = value;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            return this.GetOriginal(manager).GetVariables();
        }

        public override int GetCount(in WhileManager manager)
        {
            return this.GetIndexLookup(manager).Length;
        }

        public double GetPartPropability(in WhileManager manager)
        {
            return this.cache.GetOrCreate(nameof(GetPartPropability), manager, this.CalculatePropability);
        }

        private double CalculatePropability(in WhileManager manager)
        {
            var partPropability = 0.0;
            for (var i = 0; i < this.GetCount(manager); i++)
                partPropability += this.GetOriginal(manager).GetValue(PropabilityKey, this.GetIndexLookup(manager)[i]);

            if (partPropability == 0)
                this.cache.Update(nameof(GetIndexLookup), manager, new int[0]);
            return partPropability;
        }

        private int[] GetIndexLookup(in WhileManager manager)
        {
            return this.cache.GetOrCreate(nameof(GetIndexLookup), manager, this.CalculateIndexLookup);
        }

        private int[] CalculateIndexLookup(in WhileManager manager)
        {
            var list = new List<int>();

            var table = this.GetOriginal(manager);
            var rowCount = table.GetCount();
            for (var i = 0; i < rowCount; i++)
                if (Equals(table.GetValue(this.conditionVariable, i), this.value))
                    list.Add(i);
            var indexLookup = list.ToArray();
            return indexLookup;
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            if (p.Id == PropabilityKey.Id)
                return this.GetOriginal(manager).GetValue(PropabilityKey, this.GetIndexLookup(manager)[index]) / this.GetPartPropability(manager); // Part propability will taken into account later.

            return this.GetOriginal(manager).GetValue(p, this.GetIndexLookup(manager)[index]);
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => this.GetOriginal(manager).Contains(key);
    }

}
