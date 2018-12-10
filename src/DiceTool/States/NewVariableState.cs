using Dice.Tables;

namespace Dice.States
{
    internal class NewVariableState<T> : State
    {
        private readonly SingelVariableTable<T> table;
        private uint Id { get; }

        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new SingelVariableTable<T>(variable, distribution);
        }



        public override Table GetTable<T1>(P<T1> index)
        {
            if (index.Id == this.Id)
                return this.table;
            return base.GetTable(index);
        }
    }

}
