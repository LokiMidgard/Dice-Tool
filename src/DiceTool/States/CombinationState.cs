using System.Linq;
using System.Collections.Generic;
using System;
using Dice.Tables;
using Dice.Caches;

namespace Dice.States
{
    internal static class CombinationState
    {

    }
    internal class CombinationState<TIn1, TIn2, TOut> : TableState<CombinationTable<TIn1, TIn2, TOut>>
    {
        private readonly P<TIn1> in1;
        private readonly P<TIn2> in2;

        public override CombinationTable<TIn1, TIn2, TOut> Table { get; }
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        public CombinationState(State parent, P<TOut> variable, P<TIn1> in1, P<TIn2> in2, Func<TIn1, TIn2, TOut> func) : base(parent)
        {
            this.Table = new CombinationTable<TIn1, TIn2, TOut>(this, variable, in1, in2, func);
            this.in1 = in1;
            this.in2 = in2;
        }


        public override void PrepareOptimize(IEnumerable<IP> p)
        {
            base.PrepareOptimize(p);

            foreach (var item in p)
                this.Table.Keep(item);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.in1, this.in2 });
        }


        internal override void Optimize(in WhileManager manager)
        {
            if (this.cache.TryGet(nameof(Optimize), manager, out bool b))
                return;
            base.Optimize(manager);
            this.Table.Optimize(manager);
            this.cache.Create(nameof(Optimize), manager, true);
        }
    }
}
