using Dice.States;
using System;
using System.Collections.Generic;

namespace Dice.Tables
{
    internal class DevideTable<T> : Table
    {
        private readonly T value;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();
        //private int[]? indexLookup;
        //private double partPropability = -1;


        public State State { get; }
        public P<T> ConditionVariable { get; }

        internal (WhileManager manager, Table table) GetOriginal(in WhileManager manager) => this.State.Parent.GetTable(this.ConditionVariable, manager);

        public DevideTable(State state, P<T> p, T value)
        {
            this.State = state;
            this.ConditionVariable = p;
            this.value = value;
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
            return partPropability;
        }

        private int[] GetIndexLookup(in WhileManager manager)
        {
            return this.cache.GetOrCreate(nameof(GetIndexLookup), manager, this.CalculateIndexLookup);
        }

        private int[] CalculateIndexLookup(in WhileManager manager)
        {
            var list = new List<int>();
            for (var i = 0; i < this.GetOriginal(manager).GetCount(); i++)
                if (Equals(this.GetOriginal(manager).GetValue(this.ConditionVariable, i), this.value))
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
