using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public override void PrepareOptimize(IEnumerable<IP> ps, CancellationToken cancellation) { }

        public override bool Contains(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();


            return this.table?.Contains(variable, manager,cancellation) ?? false;
        }

        internal override void Optimize(in WhileManager manager, CancellationToken cancellation) { }

        public override double GetStatePropability(in WhileManager manager, CancellationToken cancellation) => 1.0;

        internal override State? UpdateWhileManager(ref WhileManager whileManager, CancellationToken cancellation) => null;

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();


            if (this.table?.Contains(variable, manager,cancellation) ?? false)
                return (manager, this.table!);
            throw new KeyNotFoundException($"The key with id {variable.Id} was not found.");
        }

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            if (this.table != null)
                return base.GetVarialesProvidedByThisState(cancellation).Concat(Enumerable.Repeat(this.composer.GetInput(), 1));
            return base.GetVarialesProvidedByThisState(cancellation);
        }

        internal void SetInput<TIn>(IEnumerable<TIn> input)
        {
            this.table = new SingelVariableTable<TIn>(this, ((Composer<TIn>)this.composer).GetInput(), input.Select(i => (i, 1.0)).ToArray());
        }
    }

}
