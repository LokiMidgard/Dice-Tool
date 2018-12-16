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
    internal class TransformState<TIn, TOut> : TableState<TransformTable<TIn, TOut>>
    {
        private readonly P<TOut> variable;
        private readonly P<TIn> input;
        private readonly TransformTable<TIn, TOut> table;

        public TransformState(State parent, P<TOut> variable, P<TIn> input, Func<TIn, TOut> func) : base(parent)
        {
            this.variable = variable;
            this.input = input;
            this.table = new TransformTable<TIn, TOut>(this, input, variable, func);
        }

        public override TransformTable<TIn, TOut> Table => table;


        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.input });
        }

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState()
        {
            return base.GetVarialesProvidedByThisState().Concat(new IP[] { this.variable });
        }


    }
}
