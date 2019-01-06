using System;
using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using Dice.States;

namespace Dice.Caches
{
    internal class OptimizedTableCache
    {

        private class CacheRow : IEquatable<CacheRow>
        {
            public readonly object[] columns;
            public readonly double Propability;
            private readonly OptimizedTableCache parent;

            public CacheRow(int numberOfVariables, OptimizedTableCache parent, double propability)
            {
                this.columns = new object[numberOfVariables];
                this.parent = parent;
                this.Propability = propability;
            }

            private CacheRow(CacheRow original, double propability)
            {
                this.columns = original.columns;
                this.parent = original.parent;
                this.Propability = propability;
            }

            public override bool Equals(object obj)
            {

                if (obj is CacheRow other)
                    return this.Equals(other);
                return false;

            }

            public bool Equals(CacheRow other)
            {
                if (other == null
                    || !EqualityComparer<OptimizedTableCache>.Default.Equals(this.parent, other.parent)
                    || this.columns.Length != other.columns.Length)
                    return false;



                for (var i = 0; i < this.columns.Length; i++)
                {
                    if (this.columns[i] is object[] selfArray && other.columns[i] is object[] otherArray)
                    {
                        if (selfArray.Length != otherArray.Length)
                            return false;
                        for (int j = 0; j < selfArray.Length; j++)
                        {
                            if (!Equals(selfArray[j], otherArray[j]))
                                return false;
                        }
                    }
                    else if (!this.columns[i].Equals(other.columns[i]))
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                foreach (var item in this.columns)
                {
                    if (item is object[] selfArray)
                    {
                        for (int j = 0; j < selfArray.Length; j++)
                            hash.Add(selfArray[j]);
                    }
                    else
                    {
                        hash.Add(item);
                    }

                }
                hash.Add(this.parent);
                return hash.ToHashCode();
            }

            public CacheRow With(double propability) => new CacheRow(this, propability);

        }

        private readonly CacheRow[] rows;

        private readonly Dictionary<IP, int> indexLookup = new Dictionary<IP, int>();


        public int Count => this.rows.Length;
        public object this[IP key, int index]
        {
            get
            {
                if (key.Id == Table.PropabilityKey.Id)
                {
                    return this.rows[index].Propability;
                }
                return this.rows[index].columns[this.indexLookup[key]];
            }
        }

        public IEnumerable<IP> Variables => this.indexLookup.Keys;

        public OptimizedTableCache(Table t, IEnumerable<IP> variablesToKeep, in WhileManager manager)
        {
            System.Diagnostics.Debug.Assert(variablesToKeep.Count() > 0);
            var count = 0;
            foreach (var item in variablesToKeep)
            {
                if (t.Contains(item, manager))
                {
                    this.indexLookup[item] = count;
                    count++;
                }
            }

            var rows = new List<CacheRow>();

            var counter = t.GetCount(manager);
            for (var i = 0; i < counter; i++)
            {
                var r = new CacheRow(this.indexLookup.Count, this, t.GetValue(Table.PropabilityKey, i, manager));

                foreach (var key in this.indexLookup.Keys)
                    r.columns[this.indexLookup[key]] = t.GetValue(key, i, manager);

                rows.Add(r);
            }

            this.rows = rows.GroupBy(x => x).Select(x => x.Key.With(x.Sum(r => r.Propability))).ToArray();


        }

        public bool Contains(IP key)
        {
            return this.indexLookup.ContainsKey(key);
        }
    }
}
