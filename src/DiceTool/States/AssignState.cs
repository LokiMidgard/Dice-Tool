using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    static class AssignState
    {
        public static AssignState<T> Create<T>(State parent, P<T> variable, P<T> value) => AssignState<T>.Create(parent, variable, value);
        //public static ref P<T> Create<T>(State parent, ref AssignState<T> newState, P<T> oldVariable) => throw new NotFiniteNumberException();
        //public static ref P<T> Create<T>(State parent, P<T> oldVariable) => throw new NotFiniteNumberException();

    }

    class AssignState<T> : State

    {
        private readonly P<T> value;
        private readonly P<T> variable;
        private readonly AssignTable<T> table;

        private AssignState(State parent, P<T> variable, P<T> value) : base(parent)
        {
            this.variable = variable;
            this.value = value;
            this.table = new AssignTable<T>(this.variable, this.value, this);
        }

        public static AssignState<T> Create(State parent, P<T> variable, P<T> value)
        {
            var newState = new AssignState<T>(parent, variable, value);
            return newState;
        }

        public override (WhileManager manager, Table table) GetTable<T1>(P<T1> index, in WhileManager manager)
        {

            if (this.table.Contains(index, manager))
                return (manager, this.table);

            return base.GetTable(index, manager);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.value });
        }
    }
}
