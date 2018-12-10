using System.Collections.Generic;

namespace Dice.Tables
{
    internal class ConstTable<T> : Table
    {
        private readonly P<T> variable;

        public override int Count => 1;
        public T Value { get; }

        public ConstTable(P<T> variable, T value)
        {
            this.variable = variable;
            this.Value = value;
        }


        public override object GetValue(IP p, int index)
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

        protected override bool InternalContains(IP key) => key.Id == this.variable.Id;
    }

}
