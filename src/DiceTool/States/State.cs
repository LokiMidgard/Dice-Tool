using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System.Collections.ObjectModel;
using System;

namespace Dice.States
{
    internal abstract class State
    {
        private readonly HashSet<IP> nededVariables = new HashSet<IP>();

        public virtual double GetStatePropability(in WhileManager manager)
        {
            return this.Parent.GetStatePropability(manager);
        }

        public virtual IComposer Composer => this.Parent!.Composer;

        public State Parent { get; }


        protected ISet<IP> NededVariables { get; }


        protected State(State parent)
        {
            this.Parent = parent;
            this.NededVariables = this.nededVariables.AsReadOnly();
        }


        public virtual (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager) => this.Parent.GetTable(variable, manager);

        public virtual bool Contains(IP variable, in WhileManager manager) => this.Parent.Contains(variable, manager);


        public virtual void PrepareOptimize(IEnumerable<IP> ps)
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
                .Except(this.GetVarialesProvidedByThisState())
                .Concat(this.GetOptimizedVariablesForParent())
                );
        }

        protected internal virtual IEnumerable<IP> GetOptimizedVariablesForParent() => Enumerable.Empty<IP>();
        protected internal virtual IEnumerable<IP> GetVarialesProvidedByThisState() => Enumerable.Empty<IP>();

        internal virtual void Optimize(in WhileManager manager)
        {
            this.Parent.Optimize(manager);
        }

        internal int Depth { get; private set; }

        internal void PreCalculatePath(in WhileManager manager)
        {
            var currentWhileManager = manager;
            var currentState = this;
            int dept = 0;
            do
            {
                currentState.Depth = dept;
                dept--;
                currentState = currentState.UpdateWhileManager(ref currentWhileManager);
            }
            while (currentState != null);

        }
        internal virtual State? UpdateWhileManager(ref WhileManager manager) => this.Parent;
    }



}
