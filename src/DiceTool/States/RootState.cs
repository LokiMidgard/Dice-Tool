using System;
using System.Collections.Generic;
using Dice.Tables;

namespace Dice.States
{
    internal class RootState : State
    {
        private readonly IComposer composer;

        public RootState(IComposer composer) : base(null!)
        {
            this.composer = composer;
        }
        public override IComposer Composer => this.composer;

        public override int DoCount => 0;
        public override int LoopRecursion => 0;
        public override int WhileCount => 0;

        public override int Depth => 0;

        public override void PrepareOptimize(IEnumerable<IP> ps, in WhileManager manager)
        {

        }
        internal override void Optimize(in WhileManager manager)
        {

        }

        public override double GetStatePropability(in WhileManager manager)
        {
            return 1.0;
        }

        public override (WhileManager manager, Table table) GetTable<T>(P<T> index, in WhileManager manager)
        {
            throw new KeyNotFoundException($"The key with id {index.Id} was not found.");
        }

    }

}
