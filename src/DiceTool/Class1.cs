using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Dice
{
    public class Calculator<TIn>
    {

        public static IExecutor<TResult, TIn> Configure<TResult>(Func<Composer<TIn>, P<TResult>> configurations)
        {
            var composer = new Composer<TIn>();
            var results = new List<(P<TResult> Variable, State lastState)>();
            do
            {
                var item = configurations(composer);
                results.Add((item, composer.CurrentState));
            } while (composer.Reset());

            composer.Optimize(results);

            return new Executor<TResult, TIn>(results);
        }

    }

    internal interface IP
    {
        uint Id { get; }
        IComposer Composer { get; }
    }
    public struct P<T> : IP, IEquatable<P<T>>
    {
        internal readonly uint Id;
        internal readonly IComposer Composer;

        private P(IComposer composer)
        {
            this.Id = composer.CreateId();
            this.Composer = composer ?? throw new ArgumentNullException(nameof(composer));
        }

        internal static P<T> Empty => default;

        uint IP.Id => this.Id;

        IComposer IP.Composer => this.Composer;

        internal static P<T> Create(IComposer composer) => new P<T>(composer);

        public override bool Equals(object obj)
        {
            return obj is P<T> && this.Equals((P<T>)obj);
        }

        public bool Equals(P<T> other)
        {
            return this.Id == other.Id &&
                   EqualityComparer<IComposer>.Default.Equals(this.Composer, other.Composer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id, this.Composer);
        }

        public static bool operator ==(P<T> p1, P<T> p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(P<T> p1, P<T> p2)
        {
            return !(p1 == p2);
        }

        //public static implicit operator P<T>(T e1) => throw new NotImplementedException();//new FixedE<T>((e1, 1));

    }


    internal abstract class Table
    {
        public static readonly P<double> PropabilityKey = P<double>.Empty;
        public abstract int Count { get; }

        protected abstract bool InternalContains(IP key);

        public bool Contains(IP key) => PropabilityKey.Id == key.Id || this.InternalContains(key);

        public T1 GetValue<T1>(P<T1> p, int index) => (T1)this.GetValue(p as IP, index);

        public abstract object GetValue(IP p, int index);
    }

    internal class SingelVariableTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly (T value, double propability)[] distribution;

        public SingelVariableTable(P<T> variable, (T value, double propability)[] distribution)
        {
            this.variable = variable;
            this.distribution = distribution;
        }

        public override int Count => this.distribution.Length;

        protected override bool InternalContains(IP key) => this.variable.Id == key.Id;

        public override object GetValue(IP p, int index)
        {
            switch (p)
            {
                case P<T> input when input.Id == this.variable.Id:
                    return this.distribution[index].value!;
                case P<double> input when input.Id == PropabilityKey.Id:
                    return this.distribution[index].propability;
                default:
                    throw new KeyNotFoundException($"Key with id {p.Id}.");
            }
        }
    }

    internal class CombinationTable<TIn1, TIn2, TOut> : Table
    {
        private readonly Table First;
        private readonly Table Seccond;
        private readonly P<TOut> ownP;
        private readonly Func<TIn1, TIn2, TOut> func;
        private HashSet<IP>? variablesToKeep;
        private Cache? cache;

        public P<TIn1> FirstCalculationVariable { get; }
        public P<TIn2> SeccondCalculationVariable { get; }

        public CombinationTable(Table first, Table seccond, P<TOut> p, P<TIn1> firstCalculation, P<TIn2> seccondCalculation, Func<TIn1, TIn2, TOut> func)
        {
            this.First = first;
            this.Seccond = seccond;
            this.ownP = p;
            this.FirstCalculationVariable = firstCalculation;
            this.SeccondCalculationVariable = seccondCalculation;
            this.func = func;
        }

        public override int Count
        {
            get
            {

                if (this.cache != null)
                    return this.cache.Count;

                return this.ParentTablesAreSame
                    ? this.First.Count
                    : this.First.Count * this.Seccond.Count;
            }
        }


        private bool ParentTablesAreSame => this.First == this.Seccond;


        public override object GetValue(IP p, int index)
        {
            if (this.cache != null)
                return this.cache[p, index];


            if (index >= this.Count)
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.Count}");

            int firstIndex;
            int secconedIndex;

            if (this.ParentTablesAreSame)
            {
                firstIndex = index;
                secconedIndex = index;
            }
            else
            {
                firstIndex = index % this.First.Count;
                secconedIndex = index / this.First.Count;
            }

            if (p.Id == PropabilityKey.Id)
            {
                if (this.ParentTablesAreSame)
                    return this.First.GetValue(p, index);
                return (this.First.GetValue(PropabilityKey, firstIndex) * this.Seccond.GetValue(PropabilityKey, secconedIndex));
            }

            if (p.Id == this.ownP.Id)
            {
                var firstValue = this.First.GetValue(this.FirstCalculationVariable, firstIndex);
                var seccondValue = this.Seccond.GetValue(this.SeccondCalculationVariable, secconedIndex);
                return this.func(firstValue, seccondValue)!;
            }

            if (this.First.Contains(p))
                return this.First.GetValue(p, firstIndex);
            if (this.Seccond.Contains(p))
                return this.Seccond.GetValue(p, secconedIndex);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key) => key.Id == this.ownP.Id ? true : this.First.Contains(key) || this.Seccond.Contains(key);

        internal void Keep(IP nededVariable)
        {
            if (this.Contains(nededVariable))
            {
                this.variablesToKeep ??= new HashSet<IP>();
                this.variablesToKeep!.Add(nededVariable);
            }
        }

        internal void Optimize()
        {
            if (this.variablesToKeep != null)
                this.cache ??= new Cache(this, this.variablesToKeep);

        }
    }


    internal class TransformTable<TFrom, TTo> : Table
    {
        private readonly Table original;
        private readonly P<TTo> pTo;
        private readonly Func<TFrom, TTo> func;
        public P<TFrom> PFrom { get; }

        public TransformTable(Table original, P<TFrom> PFrom, P<TTo> newP, Func<TFrom, TTo> func)
        {
            this.original = original;
            this.PFrom = PFrom;
            this.pTo = newP;
            this.func = func;
        }

        public override int Count => this.original.Count;


        public override object GetValue(IP p, int index)
        {
            if (p.Id == this.pTo.Id)
            {
                var originalValue = this.original.GetValue(this.PFrom, index);
                return this.func(originalValue)!;
            }

            return this.original.GetValue(p, index);
        }

        protected override bool InternalContains(IP key) => key.Id == this.pTo.Id ? true : this.original.Contains(key);
    }


    internal class DevideTable : Table
    {
        private readonly Table original;
        private readonly bool value;
        private int[]? indexLookup;
        private double partPropability = -1;
        public P<bool> ConditionVariable { get; }

        public DevideTable(Table table, P<bool> p, bool value)
        {
            this.original = table;
            this.ConditionVariable = p;
            this.value = value;
        }

        public override int Count => this.IndexLookup.Length;

        public double PartPropability
        {
            get
            {
                if (this.partPropability >= 0)
                    return this.partPropability;
                this.partPropability = 0;
                for (var i = 0; i < this.Count; i++)
                    this.partPropability += this.original.GetValue(PropabilityKey, this.IndexLookup[i]);
                return this.partPropability;
            }
        }


        private int[] IndexLookup
        {
            get
            {
                if (this.indexLookup != null)
                    return this.indexLookup;
                var list = new List<int>();
                for (var i = 0; i < this.original.Count; i++)
                {
                    if (this.original.GetValue(this.ConditionVariable, i) == this.value)
                    {
                        list.Add(i);
                    }
                }
                this.indexLookup = list.ToArray();
                return this.indexLookup;
            }
        }

        public override object GetValue(IP p, int index)
        {
            if (p.Id == PropabilityKey.Id)
                return (this.original.GetValue(PropabilityKey, this.IndexLookup[index]) / this.PartPropability); // Part propability will taken into account later.

            return this.original.GetValue(p, this.IndexLookup[index]);
        }

        protected override bool InternalContains(IP key) => this.original.Contains(key);
    }



    internal class ConstTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly T value;

        public ConstTable(P<T> variable, T value)
        {
            this.variable = variable;
            this.value = value;
        }

        public override int Count => 1;

        public override object GetValue(IP p, int index)
        {
            switch (p)
            {
                case P<T> input when input.Id == this.variable.Id:
                    return this.value!;
                case P<double> input when input.Id == PropabilityKey.Id:
                    return 1.0;
                default:
                    throw new KeyNotFoundException($"Key with id {p.Id}");
            }
        }

        protected override bool InternalContains(IP key) => key.Id == this.variable.Id;
    }



    public interface IExecutor<TResult, Tin>
    {

        IAsyncEnumerable<ResultEntry<TResult>> Calculate(Tin input);

    }
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

    internal class RootState : State
    {
        private readonly IComposer composer;

        public RootState(IComposer composer) : base(null!)
        {
            this.composer = composer;
        }
        public override IComposer Composer => this.composer;

        public override int Depth => 0;

    }

    internal class NewVariableState<T> : State
    {
        private readonly SingelVariableTable<T> table;
        private uint Id { get; }

        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new SingelVariableTable<T>(variable, distribution);
        }



        public override Table GetTable<T1>(P<T1> index)
        {
            if (index.Id == this.Id)
                return this.table;
            return base.GetTable(index);
        }
    }

    internal static class CombinationState
    {
        public static CombinationState<TIn1, TIn2, TOut> Create<TIn1, TIn2, TOut>(State parent, P<TOut> p, P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) => new CombinationState<TIn1, TIn2, TOut>(parent, p, e1, e2, func);

    }
    internal class CombinationState<TIn1, TIn2, TOut> : State
    {
        private readonly CombinationTable<TIn1, TIn2, TOut> table;

        public CombinationState(State parent, P<TOut> p, P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func) : base(parent)
        {
            this.table = new CombinationTable<TIn1, TIn2, TOut>(parent.GetTable(e1), parent.GetTable(e2), p, e1, e2, func);
        }

        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }

        public override void PrepareOptimize(IEnumerable<IP> p)
        {
            base.PrepareOptimize(p);
            foreach (var item in p)
                this.table.Keep(item);
        }

        protected override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.FirstCalculationVariable, this.table.SeccondCalculationVariable });
        }



        internal override void Optimize()
        {
            base.Optimize();
            this.table.Optimize();
        }
    }
    internal static class TransformState
    {
        public static TransformState<TIn, TOut> Create<TIn, TOut>(State parent, P<TOut> p, P<TIn> e, Func<TIn, TOut> func) => new TransformState<TIn, TOut>(parent, p, e, func);

    }
    internal class TransformState<TIn, TOut> : State
    {
        private readonly TransformTable<TIn, TOut> table;

        public TransformState(State parent, P<TOut> p, P<TIn> e, Func<TIn, TOut> func) : base(parent)
        {
            this.table = new TransformTable<TIn, TOut>(parent.GetTable(e), e, p, func);
        }

        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }
        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);

        }
        protected override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.PFrom });
        }

    }

    internal class DevideState : State
    {
        private readonly DevideTable table;

        private DevideState(State parent, P<bool> p, bool value) : base(parent)
        {
            this.table = new DevideTable(parent.GetTable(p), p, value);
        }

        public override double StatePropability => base.StatePropability * this.table.PartPropability;


        public static (DevideState trueState, DevideState falseState) Create(State parent, P<bool> p) => (new DevideState(parent, p, true), new DevideState(parent, p, false));

        public override Table GetTable<T>(P<T> index)
        {
            if (this.table.Contains(index))
                return this.table;
            return base.GetTable(index);
        }

        public override void PrepareOptimize(IEnumerable<IP> p)
        {
            base.PrepareOptimize(p);
        }

        protected override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.table.ConditionVariable });
        }
    }

    internal class NewConstState<T> : State
    {
        private readonly ConstTable<T> table;
        private uint Id { get; }

        public NewConstState(State parent, P<T> variable, T value) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new ConstTable<T>(variable, value);
        }


        public override Table GetTable<T1>(P<T1> index)
        {
            if (index.Id == this.Id)
                return this.table;
            return base.GetTable(index);
        }
    }


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
