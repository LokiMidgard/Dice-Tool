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
        private readonly TransformTable<TIn, TOut> table;

        public TransformState(State parent, P<TOut> p, P<TIn> e, Func<TIn, TOut> func) : base(parent)
        {
            this.table = new TransformTable<TIn, TOut>(parent.GetTable(e), e, p, func);
        }

        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }
        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);

        }
        protected override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.PFrom });
        }

    }
}
