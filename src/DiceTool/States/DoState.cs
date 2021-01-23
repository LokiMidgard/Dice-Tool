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

        internal DoTable? GetTable(in WhileManager manager)
        {
            if (this.cache.TryGet<DoTable>(nameof(GetTable), manager, out var value))
                return value;
            return null;
        }
        public DoState(State parent) : base(parent)
        {
        }



        public WhileState? WhileState { get; private set; }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable != null)
                return (manager, mergeTable);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                return base.GetTable(variable, newManager);
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.GetTable(variable, newManager);
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
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.GetStatePropability(newManager);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

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
            else if (this.WhileState is not null)
                return this.WhileState.ContinueState.Contains(variable, newManager);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");
        }

        internal override StateEnumerable UpdateWhileManager(ref WhileManager manager)
        {
            var choise = manager.Choise;
            manager = new WhileManager(manager);


            if (choise == 0)
                return new StateEnumerable(this.Parent);
            else if (this.WhileState is not null)
                return new StateEnumerable(this.WhileState.ContinueState);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

        }

        internal override void Optimize(in WhileManager manager)
        {
            if (this.GetTable(manager) != null)
                return;


            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                base.Optimize(newManager);
            else if (this.WhileState is not null)
                this.WhileState.ContinueState.Optimize(newManager);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");

            this.cache.Create(nameof(GetTable), manager, new DoTable(this, this.NededVariables, manager));
        }

        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);
            if (this.WhileState is not null)
                this.WhileState.ContinueState.PrepareOptimize(ps);
            else
                throw new InvalidOperationException("WhileState on DoState was not initilized.");
        }

    }
}
