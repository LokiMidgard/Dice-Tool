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
            int dept = 0;
            var queue = new Queue<State>();
            var alreadyVisitedStates = new HashSet<State>();
            queue.Enqueue(this);
            alreadyVisitedStates.Add(this);
            while (queue.TryDequeue(out var currentState))
            {

                currentState.Depth = dept;
                dept--;
                foreach (var newState in currentState.UpdateWhileManager(ref currentWhileManager))
                {
                    alreadyVisitedStates.Add(newState);
                    queue.Enqueue(newState);
                }
            }

        }
        internal virtual StateEnumerable UpdateWhileManager(ref WhileManager manager) => new StateEnumerable(this.Parent);
    }

    internal struct StateEnumerable : IEnumerable<State>
    {
        private readonly State? first;
        private readonly State? second;
        public static readonly StateEnumerable Empty = default;

        public StateEnumerable(State first, State second)
        {
            this.first = first;
            this.second = second;
        }
        public StateEnumerable(State first)
        {
            this.first = first;
            this.second = null;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<State> IEnumerable<State>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal struct Enumerator : IEnumerator<State>
        {
            private int call;
            private StateEnumerable stateEnumerables;

            public Enumerator(StateEnumerable stateEnumerables) : this()
            {
                this.stateEnumerables = stateEnumerables;
            }

            public State Current => this.call switch
            {
                0 => throw new InvalidOperationException("You need to call MoveNext() first"),
                1 => this.stateEnumerables.first ?? throw new InvalidOperationException("No more Elements"),
                2 => this.stateEnumerables.second ?? throw new InvalidOperationException("No more Elements"),
                _ => throw new InvalidOperationException("No more Elements")
            };

            object System.Collections.IEnumerator.Current => this.Current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (this.call == 0 && this.stateEnumerables.first != null)
                {
                    this.call += 1;
                    return true;
                }
                else if (this.call == 1 && this.stateEnumerables.second != null)
                {
                    this.call += 1;
                    return true;
                }
                else
                    return false;
            }

            public void Reset()
            {
                this.call = 0;
            }
        }
    }


}
