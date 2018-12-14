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

        public override int Depth => 0;

        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            
        }
        internal override void Optimize()
        {
            
        }

        public override double StatePropability => 1.0;


        public override Table GetTable<T>(P<T> index)
        {
            throw new KeyNotFoundException($"The key with id {index.Id} was not found.");
        }

    }

}
