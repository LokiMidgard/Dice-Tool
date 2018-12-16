using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override double GetStatePropability(in WhileManager manager)
        {
            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1.GetStatePropability(newManager);
            else
                return this.parent2.GetStatePropability(newManager);
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return (manager, mergeTable);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1.GetTable(variable, newManager);
            else
                return this.parent2.GetTable(variable, newManager);

        }

        internal override void Optimize(in WhileManager manager)
        {
            if (this.GetTable(manager) != null)
                return;

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                this.parent1.Optimize(newManager);
            else
                this.parent2.Optimize(newManager);

            //this.NededVariables.;

            this.cache.Create(nameof(GetTable), manager, new MergeTable(this, this.NededVariables, manager));


        }

        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);
            this.parent1.PrepareOptimize(ps);
            this.parent2.PrepareOptimize(ps);
        }

        internal override void PreCalculatePath(in WhileManager manager)
        {
            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                this.parent1.PreCalculatePath(newManager);
            else
                this.parent2.PreCalculatePath(newManager);
        }

        public override bool Contains(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return mergeTable.Contains(variable, manager);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return this.parent1.Contains(variable, newManager);
            else
                return this.parent2.Contains(variable, newManager);
        }







    }
}
