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
        public virtual int WhileCount => this.Parent.WhileCount;
        public virtual int DoCount => this.Parent.DoCount;

        public virtual int LoopRecursion => this.Parent.LoopRecursion;

        public virtual double GetStatePropability(in WhileManager manager)
        {
            return this.Parent.GetStatePropability(manager);
        }

        public virtual IComposer Composer => Parent!.Composer;
        public virtual int Depth => this.Parent.Depth + 1;

        public State Parent { get; }

        protected ISet<IP> NededVariables { get; }


        protected State(State parent)
        {
            this.Parent = parent;
            this.NededVariables = this.nededVariables.AsReadOnly();
        }

        public virtual double TablePropability(int index, in WhileManager manager) => this.Parent.TablePropability(index, manager);

        public virtual (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager) => this.Parent.GetTable(variable, index, manager);

        public virtual int TableCount(in WhileManager manager) => this.Parent.TableCount(manager);

        public virtual void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        {
            foreach (var p in ps)
                this.nededVariables.Add(p);
            this.Parent.PrepareOptimize(ps.Concat(this.GetOptimizedVariablesForParent()), manager);
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


    }



}
