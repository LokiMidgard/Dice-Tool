using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dice.Caches;
using Dice.States;

namespace Dice.Tables
{
    public enum OptimisationStrategy
    {
        Guess,
        NoOptimisation,
        Optimize
    }
    internal class CombinationTable<TIn1, TIn2, TOut> : Table
    {
        private OptimisationStrategy OptimisationStrategy { get; }
        private readonly CombinationState<TIn1, TIn2, TOut> state;
        private readonly P<TOut> ownP;
        private readonly Func<TIn1, TIn2, TOut> func;
        private HashSet<IP>? variablesToKeep;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();


        public P<TIn1> FirstCalculationVariable { get; }
        public P<TIn2> SeccondCalculationVariable { get; }


        public (WhileManager manager, Table table) GetFirst(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.state.Parent.GetTable(this.FirstCalculationVariable, manager, cancellation);
        }


        public (WhileManager manager, Table table) GetSeccond(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            return this.state.Parent.GetTable(this.SeccondCalculationVariable, manager, cancellation);
        }

        public CombinationTable(CombinationState<TIn1, TIn2, TOut> parent, P<TOut> p, P<TIn1> firstCalculation, P<TIn2> seccondCalculation, Func<TIn1, TIn2, TOut> func, OptimisationStrategy optimisationStrategy) : base(parent)
        {
            this.OptimisationStrategy = optimisationStrategy;
            this.state = parent;
            this.ownP = p;
            this.FirstCalculationVariable = firstCalculation;
            this.SeccondCalculationVariable = seccondCalculation;
            this.func = func;
        }

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            return this.GetParentTablesAreSame(manager, cancellation)
                ? this.GetFirst(manager, cancellation).GetCount(cancellation)
                : this.GetFirst(manager, cancellation).GetCount(cancellation) * this.GetSeccond(manager, cancellation).GetCount(cancellation);
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var firstTable = this.GetFirst(manager, cancellation);
            var seccondTable = this.GetSeccond(manager, cancellation);
            return firstTable.GetVariables(cancellation).Concat(seccondTable.GetVariables(cancellation)).Concat(Enumerable.Repeat(this.ownP as IP, 1));
        }

        private bool GetParentTablesAreSame(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var second = this.GetSeccond(manager, cancellation);
            var first = this.GetFirst(manager, cancellation);
            return first.table == second.table && Equals(first.manager, second.manager);
        }

        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager, cancellation))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager, cancellation)}");

            int firstIndex;
            int secconedIndex;

            var firstTable = this.GetFirst(manager, cancellation);
            var seccondTable = this.GetSeccond(manager, cancellation);

            var sameTables = this.GetParentTablesAreSame(manager, cancellation);
            if (sameTables)
            {
                firstIndex = index;
                secconedIndex = index;
            }
            else
            {
                firstIndex = index % firstTable.GetCount(cancellation);
                secconedIndex = index / firstTable.GetCount(cancellation);
            }

            if (p.Id == PropabilityKey.Id)
            {
                if (this.GetParentTablesAreSame(manager, cancellation))
                    return firstTable.GetValue(p, index, cancellation);
                return firstTable.GetValue(PropabilityKey, firstIndex, cancellation) * seccondTable.GetValue(PropabilityKey, secconedIndex, cancellation);
            }

            if (p.Id == this.ownP.Id)
            {
                var firstValue = firstTable.GetValue(this.FirstCalculationVariable, firstIndex, cancellation);
                var seccondValue = seccondTable.GetValue(this.SeccondCalculationVariable, secconedIndex, cancellation);
                return this.func(firstValue, seccondValue)!;
            }

            var inFirstTable = firstTable.Contains(p, cancellation);
            var inSeccondTable = seccondTable.Contains(p, cancellation);
            if (inFirstTable && (sameTables || !inSeccondTable))
                return firstTable.GetValue(p, firstIndex, cancellation);
            if (inSeccondTable && (sameTables || !inFirstTable))
                return seccondTable.GetValue(p, secconedIndex, cancellation);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);


            return key.Id == this.ownP.Id ? true : this.GetFirst(manager, cancellation).Contains(key, cancellation) || this.GetSeccond(manager, cancellation).Contains(key, cancellation);
        }

        internal void Keep(IP nededVariable)
        {
            this.variablesToKeep ??= new HashSet<IP>();
            this.variablesToKeep!.Add(nededVariable);
        }

        OptimizedTableCache? CachedTable(in WhileManager manager)
        {
            if (this.cache.TryGet<OptimizedTableCache>(nameof(Optimize), manager, out var result))
                return result;
            return null;
        }

        internal void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.variablesToKeep != null)
            {
                // if one of both table has size one, we ommit optimizing sometimes this is good sometimes bad.
                if (this.OptimisationStrategy == OptimisationStrategy.NoOptimisation || (this.OptimisationStrategy == OptimisationStrategy.Guess && (this.GetFirst(manager, cancellation).GetCount(cancellation) == 1 || this.GetSeccond(manager, cancellation).GetCount(cancellation) == 1)))
                    return;

                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable, cancellation);
            }

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager, cancellation);
        }
    }





    internal class TableCombinationTable : Table
    {
        //private readonly (WhileManager manager, Table table) firstTable;
        //private readonly (WhileManager manager, Table table) seccondTable;
        private HashSet<IP>? variablesToKeep;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();
        private readonly (WhileManager manager, Table table)[] tables;

        public (WhileManager manager, Table table) GetTable(in WhileManager manager, int index)
        {
            return this.tables[index];
        }



        public TableCombinationTable(State state, params (WhileManager manager, Table table)[] tables) : base(state)
        {
            this.tables = tables.Distinct().OrderByDescending(x => x.table.State.Depth).ToArray();
        }

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            int count = 1;

            for (int i = 0; i < this.tables.Length; i++)
            {
                count *= this.GetTable(manager, i).GetCount(cancellation);
            }
            return count;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Variables;

            var enumerable = Enumerable.Empty<IP>();
            for (int i = 0; i < this.tables.Length; i++)
                enumerable = enumerable.Concat(this.GetTable(manager, i).GetVariables(cancellation));
            return enumerable;
        }

        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager, cancellation))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager, cancellation)}");

            int[] tableIndex = new int[this.tables.Length];

            for (int i = 0; i < tableIndex.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                var previousCount = 1;
                for (int j = 0; j < i; j++)
                    previousCount *= this.GetTable(manager, j).GetCount(cancellation);

                tableIndex[i] = (index / previousCount) % this.GetTable(manager, i).GetCount(cancellation);
            }


            if (p.Id == PropabilityKey.Id)
            {
                var propability = 1.0;
                for (int i = 0; i < tableIndex.Length; i++)
                {
                    cancellation.ThrowIfCancellationRequested();
                    propability *= this.GetTable(manager, i).GetValue(PropabilityKey, tableIndex[i], cancellation);
                }

                return propability;
            }

            for (int i = 0; i < tableIndex.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                var table = this.GetTable(manager, i);
                if (table.Contains(p, cancellation))
                    return table.GetValue(p, tableIndex[i], cancellation);
            }

            throw new KeyNotFoundException($"Key with id {p.Id}.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);

            for (int i = 0; i < this.tables.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                if (this.GetTable(manager, i).Contains(key, cancellation))
                    return true;
            }
            return false;
        }

        internal void Keep(IP nededVariable)
        {
            this.variablesToKeep ??= new HashSet<IP>();
            this.variablesToKeep!.Add(nededVariable);
        }

        OptimizedTableCache? CachedTable(in WhileManager manager)
        {
            if (this.cache.TryGet<OptimizedTableCache>(nameof(Optimize), manager, out var result))
                return result;
            return null;
        }

        internal void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.variablesToKeep != null)
                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable, cancellation);

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager, CancellationToken cancellation)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager, cancellation);
        }
    }
    internal class CombinationTable<T> : Table
    {
        //private readonly (WhileManager manager, Table table) firstTable;
        //private readonly (WhileManager manager, Table table) seccondTable;
        private HashSet<IP>? variablesToKeep;
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();
        private readonly P<T>[] variables;

        public P<T[]> CombinationVariable { get; }

        public (WhileManager manager, Table table) GetTable(in WhileManager manager, int index, CancellationToken cancellation)
        {
            return this.State.Parent.GetTable(this.variables[index], manager, cancellation);
        }



        public CombinationTable(State state, P<T[]> combinationVariable, params P<T>[] variables) : base(state)
        {
            this.CombinationVariable = combinationVariable;
            this.variables = variables;
        }

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            int count = 1;

            var proxyManager = manager;
            var tables = this.variables.Select(x => (table: this.State.Parent.GetTable(x, proxyManager, cancellation), variable: x)).GroupBy(x => x.table).ToArray();


            for (int i = 0; i < tables.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                count *= tables[i].Key.GetCount(cancellation);
            }
            return count;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Variables;

            return this.variables.Cast<IP>();
        }


        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager, cancellation))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager, cancellation)}");

            var proxyManager = manager;

            var tables = this.variables.Select(x => (table: this.State.Parent.GetTable(x, proxyManager, cancellation), variable: x)).GroupBy(x => x.table).ToArray();



            int[] tableIndex = new int[tables.Length];

            for (int i = 0; i < tableIndex.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                var previousCount = 1;
                for (int j = 0; j < i; j++)
                    previousCount *= tables[j].Key.GetCount(cancellation);

                tableIndex[i] = (index / previousCount) % tables[i].Key.GetCount(cancellation);
            }

            if (p.Id == this.CombinationVariable.Id)
            {
                var result = new T[this.variables.Length];
                // combine all variables
                var counter = 0;
                for (int i = 0; i < tableIndex.Length; i++)
                {
                    cancellation.ThrowIfCancellationRequested();
                    var table = tables[i].Key;

                    foreach (var v in tables[i])
                    {
                        var vIndex = (this.variables as IList<P<T>>).IndexOf(v.variable);
                        result[vIndex] = table.GetValue(v.variable, tableIndex[i], cancellation);
                        counter++;
                    }


                }
                return result;
            }

            if (p.Id == PropabilityKey.Id)
            {
                var propability = 1.0;
                for (int i = 0; i < tableIndex.Length; i++)
                    propability *= tables[i].Key.GetValue(PropabilityKey, tableIndex[i], cancellation);

                return propability;
            }

            for (int i = 0; i < tableIndex.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                var table = tables[i].Key;
                if (table.Contains(p, cancellation))
                    return table.GetValue(p, tableIndex[i], cancellation);
            }

            throw new KeyNotFoundException($"Key with id {p.Id}.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);

            if (key.Id == this.CombinationVariable.Id)
                return true;

            for (int i = 0; i < this.variables.Length; i++)
            {
                cancellation.ThrowIfCancellationRequested();
                if (this.GetTable(manager, i, cancellation).Contains(key, cancellation))
                    return true;
            }
            return false;
        }

        internal void Keep(IP nededVariable)
        {
            this.variablesToKeep ??= new HashSet<IP>();
            this.variablesToKeep!.Add(nededVariable);
        }

        OptimizedTableCache? CachedTable(in WhileManager manager)
        {
            if (this.cache.TryGet<OptimizedTableCache>(nameof(Optimize), manager, out var result))
                return result;
            return null;
        }

        internal void Optimize(in WhileManager manager, CancellationToken cancellation)
        {
            if (this.variablesToKeep != null)
                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable, cancellation);

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager, CancellationToken cancellation)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager, cancellation);
        }
    }

}
