using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetOptimizedVariablesForParent(cancellation).Concat(Enumerable.Repeat<IP>(this.Condition, 1));
        }



    }

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

        [System.Diagnostics.DebuggerHidden]
        internal int Choise => this.Manager.GetChoise(this.Depth);

    }
}
