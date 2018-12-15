using Dice.States;
using System;

namespace Dice.Tables
{
    internal class TransformTable<TFrom, TTo> : Table
    {
        private readonly TransformState<TFrom, TTo> original;
        private readonly int index;
        private readonly P<TTo> pTo;
        private readonly Func<TFrom, TTo> func;
        public P<TFrom> PFrom { get; }

        public TransformTable(TransformState<TFrom, TTo> original, int index, P<TFrom> PFrom, P<TTo> newP, Func<TFrom, TTo> func)
        {
            this.original = original;
            this.index = index;
            this.PFrom = PFrom;
            this.pTo = newP;
            this.func = func;
        }

        public override int GetCount(in WhileManager manager)
        {
            return this.original.GetTable(this.PFrom, index, manager).GetCount();
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            if (p.Id == this.pTo.Id)
            {
                var originalValue = this.original.GetTable(this.PFrom, index, manager).GetValue(this.PFrom, index);
                return this.func(originalValue)!;
            }

            return this.original.GetTable(this.PFrom, index, manager).GetValue(p, index);
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => key.Id == this.pTo.Id ? true : this.original.GetTable(this.PFrom, index, manager).Contains(key);
    }

}
