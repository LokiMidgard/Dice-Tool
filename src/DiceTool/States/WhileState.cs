using System;
using System.Collections.Generic;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    internal class WhileState : State
    {

        public DevideState<bool> EndState { get; }

        public WhileState(State parent, P<bool> condition) : base(parent)
        {
            this.Condition = condition;
            this.EndState = new DevideState<bool>(this, condition, false);
            this.ContinueState = new DevideState<bool>(this, condition, true);
        }


        public P<bool> Condition { get; }
        public DevideState<bool> ContinueState { get; }

        public override int WhileCount => base.WhileCount + 1;
        public override int LoopRecursion => base.LoopRecursion - 1;

        public override (WhileManager manager, Table table) GetTable<T>(P<T> index, in WhileManager manager)
        {
            var newManager = new WhileManager(manager, this.WhileCount, this, 0);
            return base.GetTable(index, newManager);
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            var newManager = new WhileManager(manager, this.WhileCount, this, 0);
            return base.GetStatePropability(newManager);
        }

        internal override void Optimize(in WhileManager manager)
        {
            var newManager = new WhileManager(manager, this.WhileCount, this, 0);
            base.Optimize(newManager);
        }

        public override void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        {
            var newManager = new WhileManager(manager, this.WhileCount, this, 0);
            base.PrepareOptimize(ps, newManager);
        }


    }

    internal readonly struct WhileManager : IWhileManager, IEquatable<WhileManager>
    {

        private readonly int index;
        private readonly WhileState state;
        private readonly int countModifier;
        private readonly IWhileManager Parent;
        public (int count, WhileState state) this[int index]
        {
            get
            {
                if (index == this.index)
                    return (this.Parent[index].count + this.countModifier, this.state);
                return this.Parent[index];
            }
        }

        public WhileManager(IWhileManager parent, int index, WhileState state, int countModifier)
        {
            this.Parent = parent;
            this.index = index;
            this.state = state;
            this.countModifier = countModifier;
        }

        public override bool Equals(object obj)
        {
            return obj is WhileManager && this.Equals((WhileManager)obj);
        }

        public bool Equals(WhileManager other)
        {
            return this.index == other.index &&
                   EqualityComparer<WhileState>.Default.Equals(this.state, other.state) &&
                   this.countModifier == other.countModifier &&
                   EqualityComparer<IWhileManager>.Default.Equals(this.Parent, other.Parent);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.index, this.state, this.countModifier, this.Parent);
        }

        public static bool operator ==(WhileManager manager1, WhileManager manager2)
        {
            return manager1.Equals(manager2);
        }

        public static bool operator !=(WhileManager manager1, WhileManager manager2)
        {
            return !(manager1 == manager2);
        }
    }
}
