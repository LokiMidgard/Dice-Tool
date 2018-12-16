using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    internal class WhileState : State
    {

        public DevideState<bool> EndState { get; }


        public P<bool> Condition { get; }
        public DoState DoState { get; }
        public DevideState<bool> ContinueState { get; }



        public WhileState(State parent, P<bool> condition, DoState doState) : base(parent)
        {
            this.Condition = condition;
            this.DoState = doState;
            this.EndState = new DevideState<bool>(this, condition, false);
            this.ContinueState = new DevideState<bool>(this, condition, true);
            this.DoState.RegisterWhileState(this);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(Enumerable.Repeat<IP>(this.Condition, 1));
        }



        //public override int WhileCount => base.WhileCount + 1;
        //public override int LoopRecursion => base.LoopRecursion - 1;

        //public override (WhileManager manager, Table table) GetTable<T>(P<T> variable, int index, in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    return base.GetTable(variable, index, newManager);
        //}

        //public override double TablePropability(int index, in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    return base.TablePropability(index, newManager);
        //}

        //public override double GetStatePropability(in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    return base.GetStatePropability(newManager);
        //}

        //internal override void Optimize(in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    base.Optimize(newManager);
        //}

        //public override void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    base.PrepareOptimize(ps, newManager);
        //}

        //public override int TableCount(in WhileManager manager)
        //{
        //    var newManager = new WhileManager(manager);
        //    return base.TableCount(newManager);
        //}

    }

    //[System.Diagnostics.DebuggerDisplay("{this[1]}")]
    internal readonly struct WhileManager
    {
        internal int Depth { get; }
        private ChoiseManager Manager { get; }

        public ChoiseManager.PathToGo CacheKey => this.Manager.CacheKey(this.Depth);

        public WhileManager(ChoiseManager manager)
        {
            this.Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.Depth = 0;
        }

        public WhileManager(in WhileManager original)
        {
            this.Depth = original.Depth + 1;
            this.Manager = original.Manager;
        }

        internal int Choise => this.Manager.GetChoise(this.Depth);

    }
}
