using Dice.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dice.Tables
{
    internal class ConstTable<T> : Table
    {
        private readonly P<T> variable;

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            return 1;
        }

        public T Value { get; }

        public ConstTable(State state, P<T> variable, T value) : base(state)
        {
            this.variable = variable;
            this.Value = value;
        }


        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            switch (p)
            {
                case P<T> input when input.Id == this.variable.Id:
                    return this.Value!;
                case P<double> input when input.Id == PropabilityKey.Id:
                    return 1.0;
                default:
                    throw new KeyNotFoundException($"Key with id {p.Id}");
            }
        }
        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            return Enumerable.Repeat(this.variable as IP, 1);
        }


        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation) => key.Id == this.variable.Id;
    }

}
