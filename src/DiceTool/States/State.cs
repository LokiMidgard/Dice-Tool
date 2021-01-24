using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System.Collections.ObjectModel;
using System;
using System.Threading;

namespace Dice.States
{
    internal abstract class State
    {
        private readonly HashSet<IP> nededVariables = new HashSet<IP>();

        public virtual double GetStatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.Parent.GetStatePropability(manager, cancellation);
        }

        public virtual IComposer Composer => this.Parent!.Composer;

        public State Parent { get; }


        protected ISet<IP> NededVariables { get; }


        protected State(State parent)
        {
            this.Parent = parent;
            this.NededVariables = this.nededVariables.AsReadOnly();
        }


        public virtual (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.Parent.GetTable(variable, manager, cancellation);
        }

        public virtual bool Contains(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.Parent.Contains(variable, manager, cancellation);
        }

        public virtual void PrepareOptimize(IEnumerable<IP> ps, CancellationToken cancellation)
        {
            // we only need to update again if something changed.
            if (this.nededVariables.Count > 0)
            {
                var newPs = ps.Except(this.nededVariables);
                if (!newPs.Any())
                    return;
            }

            foreach (var p in ps)
                this.nededVariables.Add(p);
            this.Parent?.PrepareOptimize(
                ps: ps
                .Except(this.GetVarialesProvidedByThisState(cancellation))
                .Concat(this.GetOptimizedVariablesForParent(cancellation))
                , cancellation);
        }

        protected internal virtual IEnumerable<IP> GetOptimizedVariablesForParent(CancellationToken cancellation) => Enumerable.Empty<IP>();
        protected internal virtual IEnumerable<IP> GetVarialesProvidedByThisState(CancellationToken cancellation) => Enumerable.Empty<IP>();

        internal virtual void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            this.Parent.Optimize(manager, cancellation);
        }

        internal int Depth { get; private set; }

        internal void PreCalculatePath(in WhileManager manager, CancellationToken cancellation)
        {
            var currentWhileManager = manager;
            var currentState = this;
            int dept = 0;
            do
            {
                cancellation.ThrowIfCancellationRequested();
                currentState.Depth = dept;
                dept--;
                currentState = currentState.UpdateWhileManager(ref currentWhileManager, cancellation);
            }
            while (currentState != null);

        }
        internal virtual State? UpdateWhileManager(ref WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.Parent;
        }
    }



}
