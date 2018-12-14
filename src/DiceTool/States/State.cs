using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System.Collections.ObjectModel;

namespace Dice.States
{
    internal abstract class State
    {
        private readonly HashSet<IP> nededVariables = new HashSet<IP>();

        public virtual double StatePropability => this.Parent.StatePropability;

        public virtual IComposer Composer => Parent!.Composer;
        public virtual int Depth => Parent.Depth + 1;

        public State Parent { get; }

        protected ISet<IP> NededVariables { get; }


        protected State(State parent)
        {
            this.Parent = parent;
            this.NededVariables = this.nededVariables.AsReadOnly();
        }


        public virtual Table GetTable<T>(P<T> index)
        {
            return this.Parent.GetTable(index);
        }


        public virtual void PrepareOptimize(IEnumerable<IP> ps)
        {
            foreach (var p in ps)
                this.nededVariables.Add(p);
            this.Parent.PrepareOptimize(ps.Concat(this.GetOptimizedVariablesForParent()));
        }

        protected internal virtual IEnumerable<IP> GetOptimizedVariablesForParent() => Enumerable.Empty<IP>();

        internal virtual void Optimize()
        {
            this.Parent.Optimize();
        }
    }

}
