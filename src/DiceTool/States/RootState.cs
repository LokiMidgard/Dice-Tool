using System;
using System.Collections.Generic;
using System.Linq;
using Dice.Tables;

namespace Dice.States
{
    internal class RootState : State
    {
        private readonly IComposer composer;
        private Table? table;

        public RootState(IComposer composer) : base(null!)
        {
            this.composer = composer;
        }
        public override IComposer Composer => this.composer;

        public override void PrepareOptimize(IEnumerable<IP> ps) { }

        public override bool Contains(IP variable, in WhileManager manager) => table?.Contains(variable, manager) ?? false;
        internal override void Optimize(in WhileManager manager) { }

        public override double GetStatePropability(in WhileManager manager) => 1.0;

        internal override void PreCalculatePath(in WhileManager whileManager) { }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            if (this.table?.Contains(variable, manager) ?? false)
                return (manager, this.table!);
            throw new KeyNotFoundException($"The key with id {variable.Id} was not found.");
        }

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState()
        {
            if (this.table != null)
                return base.GetVarialesProvidedByThisState().Concat(Enumerable.Repeat(this.composer.GetInput(), 1));
            return base.GetVarialesProvidedByThisState();
        }

        internal void SetInput<TIn>(IEnumerable<TIn> input)
        {
            this.table = new SingelVariableTable<TIn>(((Composer<TIn>)this.composer).GetInput(), input.Select(i => (i, 1.0)).ToArray());
        }
    }

}
