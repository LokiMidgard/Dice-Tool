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

        public override int LoopRecursion => base.LoopRecursion + 1;
        public override int DoCount => base.DoCount + 1;

        public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        {
            var (count, whileState, managerIndex) = this.CalculateWhileState(manager);

            if (count == 0)
                return base.GetTable(variable, index, manager);

            var newManager = new WhileManager(manager, managerIndex, whileState, -1);
            return whileState.ContinueState.GetTable(variable, index, newManager);
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
            {
                base.Optimize(manager);
            }
            else
            {
                var newManager = new WhileManager(manager, index, whileState, -1);
                whileState.ContinueState.Optimize(newManager);
            }
        }

        public override int TableCount(in WhileManager manager)
        {
            var (count, whileState, index) = this.CalculateWhileState(manager);

            if (count == 0)
                return base.TableCount(manager);
            var newManager = new WhileManager(manager, index, whileState, -1);
            return whileState.ContinueState.TableCount(newManager);
        }

        public override void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        {
            var (count, whileState, index) = this.CalculateWhileState(manager);

            if (count == 0)
            {
                base.PrepareOptimize(ps, manager);
            }
            else
            {
                var newManager = new WhileManager(manager, index, whileState, -1);
                whileState.ContinueState.PrepareOptimize(ps, newManager);
            }
        }

        public (int count, WhileState state, int index) CalculateWhileState(in WhileManager manager) => DoState.CalculateWhileState(this, manager);
        public static (int count, WhileState state, int index) CalculateWhileState(DoState state, in WhileManager manager)
        {
            var r_d = state.LoopRecursion;
            var x = state.DoCount - 1;
            var w_0 = manager[x].state;
            var r_w_0 = w_0.LoopRecursion;
            var offset = r_d - (r_w_0 + 1);
            var w_1 = manager[x + offset];
            System.Diagnostics.Debug.Assert(w_1.count >= 0);
            return (w_1.count, w_1.state, x + offset);
        }

        public static DoState CalulateDoState(WhileState state, in WhileManager manager)
        {
            var w_0 = state;
            var r_w_0 = w_0.LoopRecursion;
            var d_0 = AncestorDo(w_0, w_0.WhileCount);
            var r_d_0 = d_0.LoopRecursion;
            var offset = r_d_0 - r_w_0 - 1;
            var d_1 = AncestorDo(w_0, w_0.WhileCount + offset);

            return d_1;
        }

        public static DoState AncestorDo(State state, int doCount)
        {
            return state.Ancestors().OfType<DoState>().TakeWhile(x => x.DoCount == doCount).Last();
        }



        public static DoState CalculateOuterMostDoState(DoState state, in WhileManager manager)
        {
            var searchedDoNumber = state.DoCount - (state.LoopRecursion - 1);
            return AncestorDo(state, searchedDoNumber);

        }

    }
}
