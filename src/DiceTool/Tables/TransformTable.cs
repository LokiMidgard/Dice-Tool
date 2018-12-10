using System;

namespace Dice.Tables
{
    internal class TransformTable<TFrom, TTo> : Table
    {
        private readonly Table original;
        private readonly P<TTo> pTo;
        private readonly Func<TFrom, TTo> func;
        public P<TFrom> PFrom { get; }

        public TransformTable(Table original, P<TFrom> PFrom, P<TTo> newP, Func<TFrom, TTo> func)
        {
            this.original = original;
            this.PFrom = PFrom;
            this.pTo = newP;
            this.func = func;
        }

        public override int Count => this.original.Count;


        public override object GetValue(IP p, int index)
        {
            if (p.Id == this.pTo.Id)
            {
                var originalValue = this.original.GetValue(this.PFrom, index);
                return this.func(originalValue)!;
            }

            return this.original.GetValue(p, index);
        }

        protected override bool InternalContains(IP key) => key.Id == this.pTo.Id ? true : this.original.Contains(key);
    }

}
