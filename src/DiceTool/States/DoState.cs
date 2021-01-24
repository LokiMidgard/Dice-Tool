using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Dice.Tables;

namespace Dice.States
{
    class DoState : State
    {

        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        internal MergeTable? GetTable(in WhileManager manager)
        {
            if (this.cache.TryGet<MergeTable>(nameof(GetTable), manager, out var value))
                return value;
            return null;
        }
        public DoState(State parent) : base(parent)
        {
        }



        public WhileState? WhileState { get; private set; }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return (manager, mergeTable);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return base.GetTable(variable, newManager, cancellation);
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.GetTable(variable, newManager, cancellation);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");
        }

        internal void RegisterWhileState(WhileState whileState)
        {
            if (this.WhileState is null)
                this.WhileState = whileState;
            else
                throw new InvalidOperationException("Whilestate already set.");
        }

        public override double GetStatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.cache.GetOrCreate(nameof(GetStatePropability), manager, this.CreateStatePropability, cancellation);
        }

        private double CreateStatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.GetStatePropability(newManager, cancellation);
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.GetStatePropability(newManager, cancellation);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

        }
        public override bool Contains(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return mergeTable.Contains(variable, manager,cancellation);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.Contains(variable, newManager, cancellation);
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.Contains(variable, newManager, cancellation);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");
        }

        internal override State? UpdateWhileManager(ref WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            manager = new WhileManager(manager);


            if (choise == 0)
                return this.Parent;
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState;
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

        }

        internal override void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.GetTable(manager) != null)
                return;

            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                base.Optimize(newManager, cancellation);
            else if (this.WhileState is not null)
                this.WhileState.ContinueState.Optimize(newManager, cancellation);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

            this.cache.Create(nameof(GetTable), manager, new MergeTable(this, this.NededVariables, manager, cancellation));
        }

        public override void PrepareOptimize(IEnumerable<IP> ps, CancellationToken cancellation)
        {
            base.PrepareOptimize(ps, cancellation);
            if (this.WhileState is not null)
                this.WhileState.ContinueState.PrepareOptimize(ps, cancellation);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");
        }

    }
}
