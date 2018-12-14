using System.Linq;
using System.Collections.Generic;
using Dice.Tables;

namespace Dice.States
{
    internal class DevideState<T> : State
    {
        private readonly DevideTable<T> table;
        public T Value { get; }

        internal DevideState(State parent, P<T> p, T value) : base(parent)
        {
            this.table = new DevideTable<T>(parent.GetTable(p), p, value);
            this.Value = value;
        }

        public override double StatePropability => base.StatePropability * this.table.PartPropability;


        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }

        public override void PrepareOptimize(IEnumerable<IP> p)
        {
            base.PrepareOptimize(p);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.ConditionVariable });
        }
    }

}
