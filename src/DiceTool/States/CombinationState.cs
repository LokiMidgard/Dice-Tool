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
            this.table = new CombinationTable<TIn1, TIn2, TOut>(this, p, e1, e2, func);
        }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> index, in WhileManager manager)
        {
            if (this.table.Contains(index, manager))
                return (manager, this.table);
            return base.GetTable(index, manager);
        }

        public override void PrepareOptimize(IEnumerable<IP> p, in WhileManager manager)
        {
            base.PrepareOptimize(p, manager);
            foreach (var item in p)
                this.table.Keep(item, manager);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.FirstCalculationVariable, this.table.SeccondCalculationVariable });
        }



        internal override void Optimize(in WhileManager manager)
        {
            base.Optimize(manager);
            this.table.Optimize(manager);
        }
    }
}
