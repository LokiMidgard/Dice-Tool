using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Dice.Tables;

namespace Dice.States
{
    class MergeState : State
    {
        private readonly State parent1;
        private readonly State parent2;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        internal MergeTable? GetTable(in WhileManager manager)
        {
            if (this.cache.TryGet<MergeTable>(nameof(GetTable), manager, out var value))
                return value;
            return null;
        }


        public MergeState(State parent1, State parent2) : base(null!)
        {
            this.parent1 = parent1;
            this.parent2 = parent2;
        }

        public override double GetStatePropability(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1.GetStatePropability(newManager, cancellation);
            else
                return this.parent2.GetStatePropability(newManager, cancellation);
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var mergeTable = this.GetTable(manager);
            if (mergeTable?.Contains(variable, manager,cancellation) ?? false)
                return (manager, mergeTable);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1.GetTable(variable, newManager, cancellation);
            else
                return this.parent2.GetTable(variable, newManager, cancellation);

        }

        internal override void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.GetTable(manager) != null)
                return;

            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                this.parent1.Optimize(newManager, cancellation);
            else
                this.parent2.Optimize(newManager, cancellation);

            //this.NededVariables.;

            this.cache.Create(nameof(GetTable), manager, new MergeTable(this, this.NededVariables, manager, cancellation));


        }

        public override void PrepareOptimize(IEnumerable<IP> ps, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            base.PrepareOptimize(ps, cancellation);
            this.parent1.PrepareOptimize(ps, cancellation);
            this.parent2.PrepareOptimize(ps, cancellation);
        }

        internal override State? UpdateWhileManager(ref WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var choise = manager.Choise;
            manager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1;
            else
                return this.parent2;
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
                return this.parent1.Contains(variable, newManager, cancellation);
            else
                return this.parent2.Contains(variable, newManager, cancellation);
        }







    }
}
