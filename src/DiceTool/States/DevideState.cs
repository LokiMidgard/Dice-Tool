using System.Linq;
using System.Collections.Generic;
using Dice.Tables;

namespace Dice.States
{
    internal class DevideState : State
    {
        private readonly DevideTable table;

        private DevideState(State parent, P<bool> p, bool value) : base(parent)
        {
            this.table = new DevideTable(parent.GetTable(p), p, value);
        }

        public override double StatePropability => base.StatePropability * this.table.PartPropability;


        public static (DevideState trueState, DevideState falseState) Create(State parent, P<bool> p) => (new DevideState(parent, p, true), new DevideState(parent, p, false));

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

        protected override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.ConditionVariable });
        }
    }

}
