using Dice.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dice.Tables
{
    internal abstract class Table
    {

        public Table(State state)
        {
            this.State = state;
        }
        public static readonly P<double> PropabilityKey = P<double>.Empty;

        public State State { get; }

        public abstract int GetCount(in WhileManager manager, CancellationToken cancellation);
        protected abstract bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation);

        public bool Contains(IP key, in WhileManager manager, CancellationToken cancellation) => PropabilityKey.Id == key.Id || this.InternalContains(key, manager, cancellation);

        public T1 GetValue<T1>(P<T1> p, int index, in States.WhileManager manager, CancellationToken cancellation) => (T1)this.GetValue(p as IP, index, manager, cancellation);

        public abstract object GetValue(IP p, int index, in States.WhileManager manager, CancellationToken cancellation);

        internal abstract IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation);



    }

    internal static class TableExtensions
    {
        public static int GetCount(this in (WhileManager manager, Table table) input, CancellationToken cancellation) => input.table.GetCount(input.manager, cancellation);

        public static bool Contains(this in (WhileManager manager, Table table) input, IP key, CancellationToken cancellation) => input.table.Contains(key, input.manager, cancellation);

        public static T1 GetValue<T1>(this in (WhileManager manager, Table table) input, P<T1> p, int index, CancellationToken cancellation) => input.table.GetValue(p, index, input.manager, cancellation);

        public static object GetValue(this in (WhileManager manager, Table table) input, IP p, int index, CancellationToken cancellation) => input.table.GetValue(p, index, input.manager, cancellation);
        public static IEnumerable<IP> GetVariables(this in (WhileManager manager, Table table) input, CancellationToken cancellation) => input.table.GetVariables(input.manager, cancellation);
    }
}
