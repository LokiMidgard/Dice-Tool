using Dice.States;
using System.Collections.Generic;
using System.Linq;

namespace Dice.Tables
{
    internal class SingelVariableTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly (T value, double propability)[] distribution;

        public SingelVariableTable(State state, P<T> variable, (T value, double propability)[] distribution) : base(state)
        {
            this.variable = variable;
            this.distribution = distribution;
        }

        public override int GetCount(in WhileManager manager)
        {
            return this.distribution.Length;
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => this.variable.Id == key.Id;

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            switch (p)
            {
                case P<T> input when input.Id == this.variable.Id:
                    return this.distribution[index].value!;
                case P<double> input when input.Id == PropabilityKey.Id:
                    return this.distribution[index].propability;
                default:
                    throw new KeyNotFoundException($"Key with id {p.Id}.");
            }
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            return Enumerable.Repeat(this.variable as IP, 1);
        }
    }

}
