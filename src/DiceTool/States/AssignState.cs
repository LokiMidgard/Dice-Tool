using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Caches;
using Dice.Tables;

namespace Dice.States
{
    static class AssignState
    {
        public static AssignState<T> Create<T>(State parent, P<T> variable, P<T> value) => AssignState<T>.Create(parent, variable, value);
        //public static ref P<T> Create<T>(State parent, ref AssignState<T> newState, P<T> oldVariable) => throw new NotFiniteNumberException();
        //public static ref P<T> Create<T>(State parent, P<T> oldVariable) => throw new NotFiniteNumberException();

    }

    class AssignState<T> : TableState<AssignTable<T>>

    {
        private readonly P<T> value;
        //private readonly WhilestateCache cache;
        private readonly P<T> variable;

        private AssignState(State parent, P<T> variable, P<T> value) : base(parent)
        {
            this.variable = variable;
            this.value = value;
            this.Table = new AssignTable<T>(this.variable, this.value, this);
            //this.cache = new Caches.WhilestateCache(this);
        }

        public override AssignTable<T> Table { get; }

        public static AssignState<T> Create(State parent, P<T> variable, P<T> value)
        {
            var newState = new AssignState<T>(parent, variable, value);
            return newState;
        }


        //private AssignTable<T> GetCachedTable(int index, in WhileManager manager)
        //{
        //    if (this.cache.TryGet<AssignTable<T>>(index.ToString(), manager, out var cachedValue))
        //        return cachedValue;

        //    cachedValue = new AssignTable<T>(this.variable, this.value, this, index);
        //    this.cache.Create(index.ToString(), manager, cachedValue);
        //    return cachedValue;
        //}




        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.value });
        }
    }
}
