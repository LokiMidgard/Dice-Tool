using Dice.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.state.Parent.GetTable(this.newContent, manager, cancellation).GetCount(cancellation);
        }

        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            if (p.Id == this.variable.Id)
                return this.state.Parent.GetTable(this.newContent, manager, cancellation).GetValue(this.newContent as IP, index, cancellation);
            return this.state.Parent.GetTable(this.newContent, manager, cancellation).GetValue(p, index, cancellation);
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.state.Parent.GetTable(this.newContent, manager, cancellation).GetVariables(cancellation).Concat(Enumerable.Repeat(this.variable as IP, 1));
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return key.Id == this.variable.Id || this.state.Parent.GetTable(this.newContent, manager, cancellation).Contains(key, cancellation);
        }
    }
}
