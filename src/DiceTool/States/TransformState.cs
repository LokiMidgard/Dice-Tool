using System.Linq;
using System.Collections.Generic;
using System;
using Dice.Tables;

namespace Dice.States
{
    internal static class TransformState
    {
        public static TransformState<TIn, TOut> Create<TIn, TOut>(State parent, P<TOut> p, P<TIn> e, Func<TIn, TOut> func) => new TransformState<TIn, TOut>(parent, p, e, func);

    }
    internal class TransformState<TIn, TOut> : State
    {
        private readonly P<TOut> variable;
        private readonly P<TIn> input;
        private readonly Func<TIn, TOut> func;

        private readonly Caches.WhilestateCache cache;

        //private readonly TransformTable<TIn, TOut>[] table;

        public TransformState(State parent, P<TOut> variable, P<TIn> input, Func<TIn, TOut> func) : base(parent)
        {
            this.variable = variable;
            this.input = input;
            this.func = func;
            this.cache = new Caches.WhilestateCache(this);
        }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        {
            if (this.variable.Id == variable.Id || this.Parent.GetTable(this.input, index, manager).table.Contains(variable, manager))
            {
                if (this.cache.TryGet<TransformTable<TIn, TOut>>(index.ToString(), manager, out var cachedValue))
                    return (manager, cachedValue);

                cachedValue = new TransformTable<TIn, TOut>(this, index,this.input,  this.variable, this.func);
                this.cache.Create(index.ToString(), manager, cachedValue);
                return (manager, cachedValue);
            }

            return base.GetTable(variable, index, manager);
        }
        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { input });
        }

    }
}
