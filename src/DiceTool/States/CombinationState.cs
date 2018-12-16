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
        //private readonly P<TOut> variable;
        private readonly P<TIn1> in1;
        private readonly P<TIn2> in2;

        public override CombinationTable<TIn1, TIn2, TOut> Table { get; }
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        //private readonly Func<TIn1, TIn2, TOut> func;
        //private readonly WhilestateCache cache;



        public CombinationState(State parent, P<TOut> variable, P<TIn1> in1, P<TIn2> in2, Func<TIn1, TIn2, TOut> func) : base(parent)
        {
            this.Table = new CombinationTable<TIn1, TIn2, TOut>(this, variable, in1, in2, func);
            //this.variable = variable;
            this.in1 = in1;
            this.in2 = in2;
            //this.func = func;
            //this.cache = new Caches.WhilestateCache(this);
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


        //private CombinationTable<TIn1, TIn2, TOut> GetCachedTable(int index, in WhileManager manager)
        //{
        //    if (this.cache.TryGet<CombinationTable<TIn1, TIn2, TOut>>(index.ToString(), manager, out var cachedValue))
        //        return cachedValue;

        //    cachedValue = new CombinationTable<TIn1, TIn2, TOut>(this, index, this.variable, this.in1, this.in2, this.func);
        //    this.cache.Create(index.ToString(), manager, cachedValue);
        //    return cachedValue;
        //}



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
