using System.Linq;
using System.Collections.Generic;
using System;
using Dice.Tables;

namespace Dice.States
{
    internal static class CombinationState
    {

    }
    internal class CombinationState<TIn1, TIn2, TOut> : State
    {
        private readonly CombinationTable<TIn1, TIn2, TOut> table;

        public CombinationState(State parent, P<TOut> p, P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) : base(parent)
        {
            this.table = new CombinationTable<TIn1, TIn2, TOut>(parent.GetTable(e1), parent.GetTable(e2), p, e1, e2, func);
        }

        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }

        public override void PrepareOptimize(IEnumerable<IP> p)
        {
            base.PrepareOptimize(p);
            foreach (var item in p)
                this.table.Keep(item);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.FirstCalculationVariable, this.table.SeccondCalculationVariable });
        }



        internal override void Optimize()
        {
            base.Optimize();
            this.table.Optimize();
        }
    }
}
