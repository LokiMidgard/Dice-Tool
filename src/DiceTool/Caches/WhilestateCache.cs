using Dice.States;
using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Dice.Caches
{
    class WhilestateCache
    {
        private readonly Dictionary<(string key, ChoiseManager.PathToGo array), object> lookup;

        //private readonly static ConditionalWeakTable<Table, WhilestateCache> store = new ConditionalWeakTable<Table, WhilestateCache>();
        public WhilestateCache()
        {
            this.lookup = new Dictionary<(string key, ChoiseManager.PathToGo array), object>();
        }

        public delegate T CreateInstanceDelegate<T>(in WhileManager manager, CancellationToken cancellation);

        public T GetOrCreate<T>(string key, in WhileManager manager, CreateInstanceDelegate<T> createion, CancellationToken cancellation)
        {
            //throw new NotImplementedException();
            var look = this.GetLookupKey(key, manager);

            if (this.lookup.ContainsKey(look))
            {
                return (T)this.lookup[look];
            }
            else
            {
                var toStore = createion(manager, cancellation);
                this.lookup.Add(look, toStore!);

                return toStore;
            }
        }

        private (string key, ChoiseManager.PathToGo array) GetLookupKey(string key, in WhileManager manager)
        {
            //(string key, int[] array) look;

            var cacheKey = manager.CacheKey;
            return (key, cacheKey);
        }

        public void Create<T>(string key, in WhileManager manager, T toStore)
        {
            var look = this.GetLookupKey(key, manager);

            if (this.lookup.ContainsKey(look))
            {
                var debug = this.lookup[look];
                throw new ArgumentException("already Added.");
            }

            this.lookup.Add(look, toStore!);
        }
        public void Update<T>(string key, in WhileManager manager, T toStore)
        {
            var look = this.GetLookupKey(key, manager);

            if (!this.lookup.ContainsKey(look))
                throw new ArgumentException("Not yet Added.");

            this.lookup[look] = toStore!;
        }

        public bool TryGet<T>(string key, in WhileManager manager, out T obj)
        {
            //throw new NotImplementedException();
            var look = this.GetLookupKey(key, manager);

            if (this.lookup.ContainsKey(look))
            {
                obj = (T)this.lookup[look];
                return true;
            }
            else
            {
                obj = default!;
                return false;
            }
        }


        private class Comparer : IEqualityComparer<(string key, ChoiseManager.PathToGo array)>
        {


            public bool Equals((string key, ChoiseManager.PathToGo array) x, (string key, ChoiseManager.PathToGo array) y)
            {
                if (x.array.Count != y.array.Count)
                    return false;
                if (x.key != y.key)
                    return false;
                return x.array.SequenceEqual(y.array);
            }


            public int GetHashCode((string key, ChoiseManager.PathToGo array) obj)
            {
                var h = new HashCode();
                h.Add(obj.array.Count);
                h.Add(obj.key);
                for (int i = 0; i < obj.array.Count; i++)
                    h.Add(obj.array[i]);
                return h.ToHashCode();
            }
        }
    }
}
