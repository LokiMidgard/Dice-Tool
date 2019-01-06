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

        //bool CreateDevideState(P<bool> p);
        P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func);
        P<T> CreateVariableState<T>(params (T value, double propability)[] distribution);

        //P<T> AssigneNameState<T>(string name, P<T> value);
        P<T> GetNamed<T>(string name);
        IP GetInput();


    }

    public partial class Composer<TInput> : IComposer
    {
        private readonly RootState root;
        private readonly Dictionary<string, Type> variableTypeMapping = new Dictionary<string, Type>();
        private readonly P<TInput> input;

        internal StateAdministrator State { get; }



        public Composer()
        {
            this.input = new P<TInput>(this, "!");
            this.root = new RootState(this);
            this.State = new StateAdministrator(this.root);
        }


        public void DoWhile(Func<P<bool>> @do)
        {
            var doState = new DoState(this.State.Current);
            this.State.NextStates(doState);
            var condition = @do();

            this.State.NextStates(new WhileState(this.State.Current, condition, doState).EndState);
        }

        internal void Setinput<TIn>(IEnumerable<TIn> input) => this.root.SetInput(input);

        public void If(P<bool> condition, Action then, Action? @else = null)
        {
            var trueState = new DevideState<bool>(this.State.Current, condition, true);
            var falseState = new DevideState<bool>(this.State.Current, condition, false);
            var states = this.State.NextStates(trueState, falseState);

            foreach (var currentState in states)
            {

                if (currentState == trueState)
                    then();
                else
                    @else?.Invoke();
            }
        }

        public P<TInput> GetInput() => this.input;

        public P<T> Const<T>(T constant) => this.CreateConstState(constant);
        public P<int> Dice(int faces) => this.Distribution(Enumerable.Range(1, faces).Select(i => (i, 1.0 / faces)).ToArray());
        public P<T> Distribution<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

        uint idCounter = 0;

        private string CreateId()
        {
            this.idCounter++;
            return this.idCounter.ToString();
        }



        public P<TOut> Combine<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            this.State.NextStates(new CombinationState<TIn1, TIn2, TOut>(this.State.Current, p, e1, e2, func));
            return p;
        }


        internal P<T> CreateConstState<T>(T value)
        {
            var p = P.Create<T>(this, this.CreateId());
            this.State.NextStates(new NewConstState<T>(this.State.Current, p, value));
            return p;
        }

        internal P<T> CreateVariableState<T>(params (T value, double propability)[] distribution)
        {
            var p = P.Create<T>(this, this.CreateId());
            this.State.NextStates(new NewVariableState<T>(this.State.Current, p, distribution));
            return p;
        }

        internal P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func)
        {
            var p = P.Create<TOut>(this, this.CreateId());
            this.State.NextStates(new TransformState<TIn, TOut>(this.State.Current, p, e, func));
            return p;
        }

        public P<T[]> Combine<T>(P<T>[] input)
        {
            var p = P.Create<T[]>(this, this.CreateId());
            this.State.NextStates(new CombinationState<T>(this.State.Current, p, input));
            return p;
        }

        public P<T> AssignName<T>(string name, P<T> value)
        {
            if (this.variableTypeMapping.ContainsKey(name))
            {
                var oldType = this.variableTypeMapping[name];
                if (oldType != typeof(T))
                    throw new InvalidOperationException($"Cannot Cast {name}({oldType}) to {typeof(T)}");
            }
            else
            {
                this.variableTypeMapping.Add(name, typeof(T));
            }
            var variable = P.Create<T>(this, name);
            this.State.NextStates(AssignState.Create(this.State.Current, variable, value));
            return variable;
        }


        public P<T> GetNamed<T>(string name)
        {
            if (this.variableTypeMapping.ContainsKey(name))
            {
                var oldType = this.variableTypeMapping[name];
                if (oldType != typeof(T))
                    throw new InvalidOperationException($"Cannot Cast {name}({oldType}) to {typeof(T)}");
            }
            else
            {
                throw new InvalidOperationException($"Unknow Variable {name}({typeof(T)})");
            }
            var variable = P.Create<T>(this, name);

            return variable;
        }


        P<TOut> IComposer.CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) => this.Combine(e1, e2, func);

        P<T> IComposer.CreateConstState<T>(T value) => this.CreateConstState(value!);

        P<TOut> IComposer.CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func) => this.CreateTransformState(e, func);

        P<T> IComposer.CreateVariableState<T>(params (T value, double propability)[] distribution) => this.CreateVariableState(distribution);

        IP IComposer.GetInput() => this.GetInput();

    }

}
