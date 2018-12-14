using System.Collections.Generic;

namespace Dice.Tables
{
    internal class DevideTable<T> : Table
    {
        private readonly Table original;
        private readonly T value;
        private int[]? indexLookup;
        private double partPropability = -1;
        public P<T> ConditionVariable { get; }

        public DevideTable(Table table, P<T> p, T value)
        {
            this.original = table;
            this.ConditionVariable = p;
            this.value = value;
        }

        public override int Count => this.IndexLookup.Length;

        public double PartPropability
        {
            get
            {
                if (this.partPropability >= 0)
                    return this.partPropability;
                this.partPropability = 0;
                for (var i = 0; i < this.Count; i++)
                    this.partPropability += this.original.GetValue(PropabilityKey, this.IndexLookup[i]);
                return this.partPropability;
            }
        }


        private int[] IndexLookup
        {
            get
            {
                if (this.indexLookup != null)
                    return this.indexLookup;
                var list = new List<int>();
                for (var i = 0; i < this.original.Count; i++)
                {
                    if (Equals(this.original.GetValue(this.ConditionVariable, i), this.value))
                    {
                        list.Add(i);
                    }
                }
                this.indexLookup = list.ToArray();
                return this.indexLookup;
            }
        }

        public override object GetValue(IP p, int index)
        {
            if (p.Id == PropabilityKey.Id)
                return this.original.GetValue(PropabilityKey, this.IndexLookup[index]) / this.PartPropability; // Part propability will taken into account later.

            return this.original.GetValue(p, this.IndexLookup[index]);
        }

        protected override bool InternalContains(IP key) => this.original.Contains(key);
    }

}
