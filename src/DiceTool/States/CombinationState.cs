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
    internal class CombinationState<TIn1, TIn2, TOut> : State
    {
        private readonly P<TOut> variable;
        private readonly P<TIn1> in1;
        private readonly P<TIn2> in2;
        private readonly Func<TIn1, TIn2, TOut> func;
        private readonly WhilestateCache cache;

        //private readonly CombinationTable<TIn1, TIn2, TOut> table;

        public CombinationState(State parent, P<TOut> variable, P<TIn1> in1, P<TIn2> in2, Func<TIn1, TIn2, TOut> func) : base(parent)
        {
            //this.table = new CombinationTable<TIn1, TIn2, TOut>(this, variable, in1, in2, func);
            this.variable = variable;
            this.in1 = in1;
            this.in2 = in2;
            this.func = func;
            this.cache = new Caches.WhilestateCache(this);
        }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        {

            if (variable.Id == this.variable.Id
                || this.Parent.GetTable(this.in1, index, manager).Contains(variable)
                || this.Parent.GetTable(this.in2, index, manager).Contains(variable))
                return (manager, this.GetCachedTable(index, manager));
            return base.GetTable(variable, index, manager);
        }

        public override void PrepareOptimize(IEnumerable<IP> p, in WhileManager manager)
        {
            base.PrepareOptimize(p, manager);

            for (int i = 0; i < this.TableCount(manager); i++)
            {
                foreach (var item in p)
                    this.GetCachedTable(i, manager).Keep(item, manager);
            }
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.in1, this.in2 });
        }


        private CombinationTable<TIn1, TIn2, TOut> GetCachedTable(int index, in WhileManager manager)
        {
            if (this.cache.TryGet<CombinationTable<TIn1, TIn2, TOut>>(index.ToString(), manager, out var cachedValue))
                return cachedValue;

            cachedValue = new CombinationTable<TIn1, TIn2, TOut>(this, index, this.variable, this.in1, this.in2, this.func);
            this.cache.Create(index.ToString(), manager, cachedValue);
            return cachedValue;
        }



        internal override void Optimize(in WhileManager manager)
        {
            base.Optimize(manager);
            for (int i = 0; i < TableCount(manager); i++)
                this.GetCachedTable(i, manager).Optimize(manager);
        }
    }
}
