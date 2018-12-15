using Dice.States;
using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Dice.Caches
{
    class WhilestateCache
    {
        private readonly State state;
        private readonly Dictionary<(string key, int[] array), object> lookup;

        //private readonly static ConditionalWeakTable<Table, WhilestateCache> store = new ConditionalWeakTable<Table, WhilestateCache>();
        public WhilestateCache(State state)
        {
            this.state = state;
            this.lookup = new Dictionary<(string key, int[] array), object>(new Comparer());
        }

        public bool TryGet<T>(string key, in WhileManager manager, out T result)
        {
            //throw new NotImplementedException();
            var look = this.GetLookupKey(key, manager);

            if (this.lookup.ContainsKey(look))
            {
                result = (T)this.lookup[look];
                return true;
            }
            else
            {
                result = default!;
                return false;
            }
        }

        private (string key, int[] array) GetLookupKey(string key, in WhileManager manager)
        {
            (string key, int[] array) look;
            if (this.state.DoCount > 0)
            {
                var doState = DoState.AncestorDo(this.state, this.state.DoCount);
                doState = DoState.CalculateOuterMostDoState(doState, manager);
                var (count, whileState, index) = DoState.CalculateWhileState(doState, manager);

                look = (key, new int[index]);
                for (int i = 0; i < look.array.Length; i++)
                    look.array[i] = manager[i].count;
            }
            else
            {
                look = (key, new int[0]);
            }

            return look;
        }

        public void Create<T>(string key, in WhileManager manager, T toStore)
        {
            var look = this.GetLookupKey(key, manager);

            if (this.lookup.ContainsKey(look))
                throw new ArgumentException("already Added.");

            this.lookup.Add(look, toStore!);
        }

        private class Comparer : IEqualityComparer<(string key, int[] array)>
        {


            public bool Equals((string key, int[] array) x, (string key, int[] array) y)
            {
                if (x.array.Length != y.array.Length)
                    return false;
                if (x.key != y.key)
                    return false;
                return x.array.SequenceEqual(y.array);
            }


            public int GetHashCode((string key, int[] array) obj)
            {
                var h = new HashCode();
                h.Add(obj.array.Length);
                h.Add(obj.key);
                for (int i = 0; i < obj.array.Length; i++)
                    h.Add(obj.array[i]);
                return h.ToHashCode();
            }
        }
    }
}
