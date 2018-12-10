namespace Dice.Tables
{
    internal abstract class Table
    {
        public static readonly P<double> PropabilityKey = P<double>.Empty;
        public abstract int Count { get; }

        protected abstract bool InternalContains(IP key);

        public bool Contains(IP key) => PropabilityKey.Id == key.Id || this.InternalContains(key);

        public T1 GetValue<T1>(P<T1> p, int index) => (T1)this.GetValue(p as IP, index);

        public abstract object GetValue(IP p, int index);
    }

}
