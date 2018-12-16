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

    }

    class AssignState<T> : TableState<AssignTable<T>>

    {
        private readonly P<T> value;
        private readonly P<T> variable;

        private AssignState(State parent, P<T> variable, P<T> value) : base(parent)
        {
            this.variable = variable;
            this.value = value;
            this.Table = new AssignTable<T>(this.variable, this.value, this);
        }

        public override AssignTable<T> Table { get; }

        public static AssignState<T> Create(State parent, P<T> variable, P<T> value)
        {
            var newState = new AssignState<T>(parent, variable, value);
            return newState;
        }


        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.value });
        }
    }
}
