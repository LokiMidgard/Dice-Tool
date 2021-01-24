using Dice.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dice.Tables
{
    internal class TransformTable<TFrom, TTo> : Table
    {
        private readonly TransformState<TFrom, TTo> state;
        private readonly P<TTo> pTo;
        private readonly Func<TFrom, TTo> func;
        private readonly P<TFrom> pFrom;

        public TransformTable(TransformState<TFrom, TTo> original, P<TFrom> PFrom, P<TTo> newP, Func<TFrom, TTo> func) : base(original)
        {
            this.state = original;
            this.pFrom = PFrom;
            this.pTo = newP;
            this.func = func;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            return this.state.Parent.GetTable(this.pFrom, manager, cancellation).GetVariables(cancellation).Concat(Enumerable.Repeat(this.pTo as IP, 1));
        }

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            return this.state.Parent.GetTable(this.pFrom, manager, cancellation).GetCount(cancellation);
        }

        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            if (p.Id == this.pTo.Id)
            {
                var originalValue = this.state.Parent.GetTable(this.pFrom, manager, cancellation).GetValue(this.pFrom, index, cancellation);
                return this.func(originalValue)!;
            }

            return this.state.Parent.GetTable(this.pFrom, manager, cancellation).GetValue(p, index, cancellation);
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return key.Id == this.pTo.Id ? true : this.state.Parent.GetTable(this.pFrom, manager, cancellation).Contains(key, cancellation);
        }
    }

}
