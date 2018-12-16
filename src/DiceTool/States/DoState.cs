using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    class DoState : State
    {
        public DoState(State parent) : base(parent)
        {
        }



        public WhileState WhileState { get; private set; }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, in WhileManager manager)
        {

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
            //var (count, whileState, index) = this.CalculateWhileState(manager);

            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.GetStatePropability(newManager);
            else
                return this.WhileState.ContinueState.GetStatePropability(newManager);
        }


        public override bool Contains(IP variable, in WhileManager manager)
        {
            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                return base.Contains(variable, newManager);
            else
                return this.WhileState.ContinueState.Contains(variable, newManager);
        }

        internal override void PreCalculatePath(in WhileManager manager)
        {
            var choise = manager.Choise;
            var newManager = new WhileManager(manager);


            if (choise == 0)
                base.PreCalculatePath(newManager);
            else
                this.WhileState.ContinueState.PreCalculatePath(newManager);

        }

        internal override void Optimize(in WhileManager manager)
        {
            //var (count, whileState, index) = this.CalculateWhileState(manager);
            var choise = manager.Choise;
            var newManager = new WhileManager(manager);

            if (choise == 0)
                base.Optimize(newManager);
            else
                this.WhileState.ContinueState.Optimize(newManager);

        }

        
        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);
            this.WhileState.ContinueState.PrepareOptimize(ps);
        }




    }
}
