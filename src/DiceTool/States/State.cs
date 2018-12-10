using System.Linq;
using System.Collections.Generic;
using Dice.Tables;

namespace Dice.States
{
    internal abstract class State
    {
        protected State(State parent)
        {
            this.Parent = parent;
        }

        public virtual double StatePropability => this.Parent?.StatePropability ?? 1.0;


        public virtual IComposer Composer => Parent!.Composer;
        public virtual int Depth => Parent!.Depth + 1;

        public State? Parent { get; }

        public virtual Table GetTable<T>(P<T> index)
        {
            return this.Parent?.GetTable(index) ?? throw new KeyNotFoundException($"The key with id {index.Id} was not found.");
        }

        protected readonly HashSet<IP> nededVariables = new HashSet<IP>();

        public virtual void PrepareOptimize(IEnumerable<IP> ps)
        {
            foreach (var p in ps)
                this.nededVariables.Add(p);
            this.Parent?.PrepareOptimize(ps.Concat(this.GetOptimizedVariablesForParent()));
        }

        protected virtual IEnumerable<IP> GetOptimizedVariablesForParent() => Enumerable.Empty<IP>();



        internal virtual void Optimize()
        {
            this.Parent?.Optimize();
        }
    }

}
