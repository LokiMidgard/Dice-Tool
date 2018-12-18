﻿using Dice.States;
using System;

namespace Dice.Tables
{
    internal class TransformTable<TFrom, TTo> : Table
    {
        private readonly TransformState<TFrom, TTo> state;
        private readonly P<TTo> pTo;
        private readonly Func<TFrom, TTo> func;
        private readonly P<TFrom> pFrom;

        public TransformTable(TransformState<TFrom, TTo> original, P<TFrom> PFrom, P<TTo> newP, Func<TFrom, TTo> func)
        {
            this.state = original;
            this.pFrom = PFrom;
            this.pTo = newP;
            this.func = func;
        }

        public override int GetCount(in WhileManager manager)
        {
            return this.state.Parent.GetTable(this.pFrom, manager).GetCount();
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            if (p.Id == this.pTo.Id)
            {
                var originalValue = this.state.Parent.GetTable(this.pFrom, manager).GetValue(this.pFrom, index);
                return this.func(originalValue)!;
            }

            return this.state.Parent.GetTable(this.pFrom, manager).GetValue(p, index);
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => key.Id == this.pTo.Id ? true : this.state.Parent.GetTable(this.pFrom, manager).Contains(key);
    }

}
