using System;
using System.Linq;
using System.Collections.Generic;
using Dice.States;

namespace Dice
{

    internal interface IComposer
    {
        State CurrentState { get; }


        P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func);
        P<T> CreateConstState<T>(T value);
        bool CreateDevideState(P<bool> p);
        P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func);
        P<T> CreateVariableState<T>(params (T value, double propability)[] distribution);

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

        public P<T> Const<T>(T constant) => this.CreateConstState(constant);
        public P<int> Dice(int faces) => this.Distribution(Enumerable.Range(1, faces).Select(i => (i, 1.0 / faces)).ToArray());
        public P<T> Distribution<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

        uint idCounter = 1;

        private uint CreateId()
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


        internal P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            var state = new CombinationState<TIn1, TIn2, TOut>(this.CurrentState, p, e1, e2, func);
            this.NextStates(state);
            return p;
        }


        /// <summary>
        /// Creates a devine state and returns which path was taken.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal bool CreateDevideState(P<bool> p)
        {
            var trueState = new DevideState(this.CurrentState, p, true);
            var falseState = new DevideState(this.CurrentState, p, false);
            return trueState == this.NextStates(trueState, falseState);
        }

        internal P<T> CreateConstState<T>(T value)
        {
            var p = P.Create<T>(this, this.CreateId());
            var state = new NewConstState<T>(this.CurrentState, p, value);
            this.NextStates(state);
            return p;

        }

        internal P<T> CreateVariableState<T>(params (T value, double propability)[] distribution)
        {
            var p = P.Create<T>(this, this.CreateId());
            var state = new NewVariableState<T>(this.CurrentState, p, distribution);
            this.NextStates(state);
            return p;
        }

        internal P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            var state = new TransformState<TIn, TOut>(this.CurrentState, p, e, func);
            this.NextStates(state);
            return p;
        }

        P<TOut> IComposer.CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) => this.CreateCombineState(e1, e2, func);

        P<T> IComposer.CreateConstState<T>(T value) => this.CreateConstState(value!);

        bool IComposer.CreateDevideState(P<bool> p) => this.CreateDevideState(p);

        P<TOut> IComposer.CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func) => this.CreateTransformState(e, func);

        P<T> IComposer.CreateVariableState<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

        State IComposer.CurrentState => this.CurrentState;
    }
}
