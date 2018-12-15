using System;
using System.Collections.Generic;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    class DoState : State
    {
        public DoState(State parent) : base(parent)
        {
        }

        public override int LoopRecursion => base.LoopRecursion + 1;
        public override int DoCount => base.DoCount + 1;

        public override (WhileManager manager, Table table) GetTable<T>(P<T> index, in WhileManager manager)
        {
            var (count, whileState, managerIndex) = this.CalculateWhileState(manager);

            if (count == 0)
                return base.GetTable(index, manager);

            var newManager = new WhileManager(manager, managerIndex, whileState, -1);
            return whileState.ContinueState.GetTable(index, newManager);
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            var (count, whileState, index) = this.CalculateWhileState(manager);

            if (count == 0)
                return base.GetStatePropability(manager);
            var newManager = new WhileManager(manager, index, whileState, -1);
            return whileState.ContinueState.GetStatePropability(newManager);
        }

        internal override void Optimize(in WhileManager manager)
        {
            var (count, whileState, index) = this.CalculateWhileState(manager);

            if (count == 0)
                base.Optimize(manager);
            var newManager = new WhileManager(manager, index, whileState, -1);
            whileState.ContinueState.Optimize(newManager);
        }

        public override void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        {
            var (count, whileState, index) = this.CalculateWhileState(manager);

            if (count == 0)
                base.PrepareOptimize(ps, manager);
            var newManager = new WhileManager(manager, index, whileState, -1);
            whileState.ContinueState.PrepareOptimize(ps, newManager);
        }

        private (int count, WhileState state, int index) CalculateWhileState(in WhileManager manager)
        {
            var r_d = this.LoopRecursion;
            var x = this.DoCount;
            var w_0 = manager[x].state;
            var r_w_0 = w_0.LoopRecursion;
            var offset = r_d - (r_w_0 + 1);
            var w_1 = manager[x + offset];
            return (w_1.count, w_1.state, x + offset);
        }
    }
}
