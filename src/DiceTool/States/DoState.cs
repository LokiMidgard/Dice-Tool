using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



        public WhileState WhileState { get; private set; }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return (manager, mergeTable);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return base.GetTable(variable, newManager);
            else
                return this.WhileState.ContinueState.GetTable(variable, newManager);
        }

        internal void RegisterWhileState(WhileState whileState)
        {
            if (this.WhileState is null)
                this.WhileState = whileState;
            else
                throw new InvalidOperationException("Whilestate already set.");
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            return this.cache.GetOrCreate(nameof(GetStatePropability), manager, this.CreateStatePropability);

        }

        private double CreateStatePropability(in WhileManager manager)
        {

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.GetStatePropability(newManager);
            else
                return this.WhileState.ContinueState.GetStatePropability(newManager);

        }
        public override bool Contains(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return mergeTable.Contains(variable, manager);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.Contains(variable, newManager);
            else
                return this.WhileState.ContinueState.Contains(variable, newManager);
        }

        internal override State? UpdateWhileManager(ref WhileManager manager)
        {
            var choise = manager.Choise;
            manager = new WhileManager(manager);


            if (choise == 0)
                return this.Parent;
            else
                return this.WhileState.ContinueState;

        }

        internal override void Optimize(in WhileManager manager)
        {
            if (this.GetTable(manager) != null)
                return;


            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                base.Optimize(newManager);
            else
                this.WhileState.ContinueState.Optimize(newManager);

            this.cache.Create(nameof(GetTable), manager, new MergeTable(this, this.NededVariables, manager));

        }


        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);
            this.WhileState.ContinueState.PrepareOptimize(ps);
        }




    }
}
