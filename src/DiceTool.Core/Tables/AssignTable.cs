using Dice.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dice.Tables
{
    class AssignTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly P<T> newContent;
        private readonly AssignState<T> state;

        public AssignTable(P<T> variable, P<T> newContent, AssignState<T> state) : base(state)
        {
            this.variable = variable;
            this.newContent = newContent;
            this.state = state;
        }

        public override int GetCount(in WhileManager manager)
        {
            return this.state.Parent.GetTable(this.newContent, manager).GetCount();
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            if (p.Id == this.variable.Id)
                return this.state.Parent.GetTable(this.newContent, manager).GetValue(this.newContent as IP, index);
            return this.state.Parent.GetTable(this.newContent, manager).GetValue(p, index);
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            return this.state.Parent.GetTable(this.newContent, manager).GetVariables().Concat(Enumerable.Repeat(this.variable as IP, 1));
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => key.Id == this.variable.Id || this.state.Parent.GetTable(this.newContent, manager).Contains(key);

    }
}
