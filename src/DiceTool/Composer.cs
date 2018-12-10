using System;
using System.Linq;
using System.Collections.Generic;
using Dice.States;

namespace Dice
{

    internal interface IComposer
    {
        State CurrentState { get; }

        State NextStates(params State[] states);

        uint CreateId();
    }

    public class Composer<TInput> : IComposer
    {
        private readonly RootState root;

        internal State CurrentState { get; set; }

        public Composer()
        {
            this.root = new RootState(this);
            this.CurrentState = this.root;
        }


        [Obsolete("Not Yet implemented", true)]
        public void DoWhile(Func<P<bool>> @do)
        {
            throw new NotImplementedException();
        }

        public P<T> Const<T>(T constant)
        {
            var p = P<T>.Create(this);
            var newState = new NewConstState<T>(this.CurrentState, p, constant);
            this.CurrentState = newState;
            return p;
        }
        public P<int> Dice(int faces) => this.Distribution(Enumerable.Range(1, faces).Select(i => (i, 1.0 / faces)).ToArray());
        public P<T> Distribution<T>(params (T value, double propability)[] distribution)
        {
            var p = P<T>.Create(this);
            var newState = new NewVariableState<T>(this.CurrentState, p, distribution);
            this.CurrentState = newState;
            return p;
        }

        uint idCounter = 1;

        internal uint CreateId()
        {
            return this.idCounter++;
        }

        /// <summary>
        /// Restes all counter so a different path can searched.
        /// </summary>
        /// <returns><c>true</c> if not all possible pathes have been searched.</returns>
        internal bool Reset()
        {
            this.idCounter = 1;

            this.lastPath = this.currentPath;
            while (this.lastPath != null && this.lastPath?.choise == this.lastPath?.maxChoises - 1)
                this.lastPath = this.lastPath.parent; // remove completed pathes

            this.currentPath = null;
            this.CurrentState = this.root;

            return this.lastPath != null; // we have no last path, either we never encuntered a fork, or we have calculated the last posible way.
        }

        internal void Optimize<TResult>(IEnumerable<(P<TResult> result, State state)> resultData)
        {
            foreach (var (p, state) in resultData)
                state.PrepareOptimize(Enumerable.Repeat(p, 1).Cast<IP>());

            foreach (var (_, state) in resultData)
                state.Optimize();


        }

        State NextStates(params State[] states)
        {
            State stateToReturn;
            if (states.Length == 1)
            {
                stateToReturn = states[0];
            }
            else
            {
                var numberOfChoises = states.Length;

                if (this.lastPath != null)
                {

                    if (this.lastPath.depth == this.CurrentState.Depth)
                    {
                        // we are at position when last time we did the last decision
                        // so now we need to do another decision.
                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = this.lastPath.choise + 1,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };
                    }
                    else if (this.lastPath.depth > this.CurrentState.Depth)
                    {
                        // We need to follow the last path further
                        var current = this.lastPath;
                        while (current.depth != this.CurrentState.Depth)
                            current = current.parent ?? throw new InvalidOperationException("Did not find correct parent");

                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = current.choise,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };
                    }
                    else
                    {
                        // this is the first time we got here
                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = 0,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };

                    }
                    stateToReturn = states[this.currentPath.choise];
                }
                else
                {
                    // this is the first time so we will always take the first choise
                    this.currentPath = new LastChoise()
                    {
                        depth = this.CurrentState.Depth,
                        choise = 0,
                        maxChoises = numberOfChoises,
                        parent = currentPath
                    };
                    stateToReturn = states[0];
                }



            }

            return this.CurrentState = stateToReturn;
        }

        private LastChoise? lastPath;
        private LastChoise? currentPath;

        private class LastChoise
        {
            public int depth;
            public int choise;
            public int maxChoises;
            public LastChoise? parent;
        }



        State IComposer.CurrentState => this.CurrentState;
        uint IComposer.CreateId() => this.CreateId();
        State IComposer.NextStates(params State[] states) => this.NextStates(states);
    }
}
