using Dice.Tables;

namespace Dice.States
{
    internal class NewConstState<T> : State
    {
        private readonly ConstTable<T> table;
        private string Id { get; }

        public NewConstState(State parent, P<T> variable, T value) : base(parent)
        {
            this.Id = variable.Id;
            this.table = new ConstTable<T>(variable, value);
        }


        public override Table GetTable<T1>(P<T1> index)
        {
            if (index.Id == this.Id)
                return this.table;
            return base.GetTable(index);
        }
    }

}
