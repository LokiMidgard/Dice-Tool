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

        public virtual IComposer Composer => Parent!.Composer;

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
            this.Parent?.PrepareOptimize(ps.Concat(this.GetOptimizedVariablesForParent()));
        }

        protected internal virtual IEnumerable<IP> GetOptimizedVariablesForParent() => Enumerable.Empty<IP>();

        internal virtual void Optimize(in WhileManager manager)
        {
            this.Parent.Optimize(manager);
        }

        public IEnumerable<State> Ancestors()
        {
            var current = this;
            while (true)
            {
                yield return current;
                if (current.Parent is null)
                    yield break;
                current = current.Parent;
            }
        }

        internal virtual void PreCalculatePath(in WhileManager manager) => this.Parent.PreCalculatePath(manager);
    }



}
