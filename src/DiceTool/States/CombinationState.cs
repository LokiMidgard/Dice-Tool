using System.Linq;
using System.Collections.Generic;
using System;
using Dice.Tables;
using Dice.Caches;
using System.Threading;

namespace Dice.States
{
    internal static class CombinationState
    {

    }
    internal class CombinationState<TIn1, TIn2, TOut> : TableState<CombinationTable<TIn1, TIn2, TOut>>
    {
        private readonly P<TOut> variable;
        private readonly P<TIn1> in1;
        private readonly P<TIn2> in2;

        public override CombinationTable<TIn1, TIn2, TOut> Table { get; }
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        public CombinationState(State parent, P<TOut> variable, P<TIn1> in1, P<TIn2> in2, Func<TIn1, TIn2, TOut> func, OptimisationStrategy optimisationStrategy) : base(parent)
        {
            this.Table = new CombinationTable<TIn1, TIn2, TOut>(this, variable, in1, in2, func, optimisationStrategy);
            this.variable = variable;
            this.in1 = in1;
            this.in2 = in2;
        }


        public override void PrepareOptimize(IEnumerable<IP> p, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();


            base.PrepareOptimize(p, cancellation);

            foreach (var item in p)
                this.Table.Keep(item);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetOptimizedVariablesForParent(cancellation).Concat(new IP[] { this.in1, this.in2 });
        }
        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();


            return base.GetVarialesProvidedByThisState(cancellation).Concat(new IP[] { this.variable });
        }


        internal override void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.cache.TryGet(nameof(Optimize), manager, out bool b))
                return;
            cancellation.ThrowIfCancellationRequested();

            base.Optimize(manager, cancellation);
            // If the table has more variables then we need to keep we optimize the table otherwise it will result in no compression.
            if (this.Table.GetVariables(manager, cancellation).Except(this.NededVariables).Any())
                this.Table.Optimize(manager, cancellation);
            this.cache.Create(nameof(Optimize), manager, true);
        }
    }

    internal class CombinationState<T> : TableState<CombinationTable<T>>
    {
        private readonly P<T[]> p;
        private readonly P<T>[] input;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        public CombinationState(State current, P<T[]> p, P<T>[] input) : base(current)
        {
            this.Table = new CombinationTable<T>(this, p, input);

            this.p = p;
            this.input = input;
        }

        public override CombinationTable<T> Table { get; }
        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetOptimizedVariablesForParent(cancellation).Concat(this.input.Cast<IP>());
        }
        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetVarialesProvidedByThisState(cancellation).Concat(new IP[] { this.p });
        }

        public override void PrepareOptimize(IEnumerable<IP> p, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            base.PrepareOptimize(p, cancellation);

            foreach (var item in p)
                this.Table.Keep(item);
        }
        internal override void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.cache.TryGet(nameof(Optimize), manager, out bool b))
                return;
            cancellation.ThrowIfCancellationRequested();
            base.Optimize(manager, cancellation);
            this.Table.Optimize(manager, cancellation);
            this.cache.Create(nameof(Optimize), manager, true);
        }
    }
}
