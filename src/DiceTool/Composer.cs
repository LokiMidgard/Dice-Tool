using System;
using System.Linq;
using System.Collections.Generic;
using Dice.States;

namespace Dice
{

    internal interface IComposer
    {
        P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func);
        P<T> CreateConstState<T>(T value);

        /// <summary>
        /// Creates a devine state and returns which path was taken.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>

        bool CreateDevideState(P<bool> p);
        P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func);
        P<T> CreateVariableState<T>(params (T value, double propability)[] distribution);

    }

    public partial class Composer<TInput> : IComposer
    {
        private readonly RootState root;

        internal StateAdministrator State { get; }

        private readonly ComposerWraper composerWraper;


        public Composer()
        {
            this.root = new RootState(this);
            this.State = new StateAdministrator(root);
            this.composerWraper = new ComposerWraper(this);
        }


        [Obsolete("Not Yet implemented", true)]
        public void DoWhile(Func<P<bool>> @do)
        {
            this.composerWraper.Internal = new DoComposer(this, @do);

        }

        public P<T> Const<T>(T constant) => this.CreateConstState(constant);
        public P<int> Dice(int faces) => this.Distribution(Enumerable.Range(1, faces).Select(i => (i, 1.0 / faces)).ToArray());
        public P<T> Distribution<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

        uint idCounter = 0;

        private uint CreateId()
        {
            this.idCounter++;
            return this.idCounter;
        }

        /// <summary>
        /// Restes all counter so a different path can searched.
        /// </summary>
        /// <returns><c>true</c> if not all possible pathes have been searched.</returns>
        internal bool Reset()
        {
            this.idCounter = 1;

            return this.State.MoveNext();
        }

        internal void Optimize<TResult>(IEnumerable<(P<TResult> result, State state)> resultData)
        {
            foreach (var (p, state) in resultData)
                state.PrepareOptimize(Enumerable.Repeat(p, 1).Cast<IP>());

            foreach (var (_, state) in resultData)
                state.Optimize();


        }


        internal P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            var state = new CombinationState<TIn1, TIn2, TOut>(this.State.CurrentState, p, e1, e2, func);
            this.State.NextStates(state);
            return p;
        }


        /// <summary>
        /// Creates a devine state and returns which path was taken.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal bool CreateDevideState(P<bool> p)
        {
            var trueState = new DevideState(this.State.CurrentState, p, true);
            var falseState = new DevideState(this.State.CurrentState, p, false);
            return trueState == this.State.NextStates(trueState, falseState);
        }

        internal P<T> CreateConstState<T>(T value)
        {
            var p = P.Create<T>(this, this.CreateId());
            var state = new NewConstState<T>(this.State.CurrentState, p, value);
            this.State.NextStates(state);
            return p;

        }

        internal P<T> CreateVariableState<T>(params (T value, double propability)[] distribution)
        {
            var p = P.Create<T>(this, this.CreateId());
            var state = new NewVariableState<T>(this.State.CurrentState, p, distribution);
            this.State.NextStates(state);
            return p;
        }

        internal P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            var state = new TransformState<TIn, TOut>(this.State.CurrentState, p, e, func);
            this.State.NextStates(state);
            return p;
        }


        P<TOut> IComposer.CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) => this.CreateCombineState(e1, e2, func);

        P<T> IComposer.CreateConstState<T>(T value) => this.CreateConstState(value!);

        bool IComposer.CreateDevideState(P<bool> p) => this.CreateDevideState(p);

        P<TOut> IComposer.CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func) => this.CreateTransformState(e, func);

        P<T> IComposer.CreateVariableState<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

    }

    internal class ComposerWraper : IComposer
    {
        public ComposerWraper(IComposer composer)
        {
            this.Internal = composer ?? throw new ArgumentNullException(nameof(composer));
        }

        public IComposer Internal { get; set; }


        public P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) => this.Internal.CreateCombineState(e1, e2, func);

        public P<T> CreateConstState<T>(T value) => this.Internal.CreateConstState(value);

        public bool CreateDevideState(P<bool> p) => this.Internal.CreateDevideState(p);

        public P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func) => this.Internal.CreateTransformState(e, func);

        public P<T> CreateVariableState<T>(params (T value, double propability)[] distribution) => this.Internal.CreateVariableState(distribution);
    }
}
