using System.Collections.Generic;

namespace Dice.Tables
{
    internal class SingelVariableTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly (T value, double propability)[] distribution;

        public SingelVariableTable(P<T> variable, (T value, double propability)[] distribution)
        {
            this.variable = variable;
            this.distribution = distribution;
        }

        public override int Count => this.distribution.Length;

        protected override bool InternalContains(IP key) => this.variable.Id == key.Id;

        public override object GetValue(IP p, int index)
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
    }

}
