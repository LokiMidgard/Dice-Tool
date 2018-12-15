using Dice.States;

namespace Dice.Tables
{
    internal abstract class Table
    {
        public static readonly P<double> PropabilityKey = P<double>.Empty;

        public abstract int GetCount(in WhileManager manager);
        protected abstract bool InternalContains(IP key, in WhileManager manager);

        public bool Contains(IP key, in WhileManager manager) => PropabilityKey.Id == key.Id || this.InternalContains(key, manager);

        public T1 GetValue<T1>(P<T1> p, int index, in States.WhileManager manager) => (T1)this.GetValue(p as IP, index, manager);

        public abstract object GetValue(IP p, int index, in States.WhileManager manager);

    }

    internal static class TableExtensions
    {
        public static int GetCount(this in (WhileManager manager, Table table) input) => input.table.GetCount(input.manager);

        public static bool Contains(this in (WhileManager manager, Table table) input, IP key) => input.table.Contains(key, input.manager);

        public static T1 GetValue<T1>(this in (WhileManager manager, Table table) input, P<T1> p, int index) => input.table.GetValue(p, index, input.manager);

        public static object GetValue(this in (WhileManager manager, Table table) input, IP p, int index) => input.table.GetValue(p, index, input.manager);
    }
}
