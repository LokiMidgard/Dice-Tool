using Dice.Tables;
using System.Threading;

namespace Dice.States
{
    internal abstract class TableState<T> : State
        where T : Table
    {

        public abstract T Table { get; }

        public TableState(State parent) : base(parent)
        {
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            if (this.Table.Contains(variable, manager,cancellation))
                return (manager, this.Table);
            return base.GetTable(variable, manager, cancellation);
        }

        public override bool Contains(IP variable, in WhileManager manager, CancellationToken cancellation)
        {
            if (this.Table.Contains(variable, manager, cancellation))
                return true;
            return base.Contains(variable, manager, cancellation);
        }

    }

}
