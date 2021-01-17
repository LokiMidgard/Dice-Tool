using System;
using System.Collections.Generic;
using System.Linq;
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


        public (WhileManager manager, Table table) GetFirst(in WhileManager manager)
        {
            return this.state.Parent.GetTable(this.FirstCalculationVariable, manager);
        }


        public (WhileManager manager, Table table) GetSeccond(in WhileManager manager)
        {
            return this.state.Parent.GetTable(this.SeccondCalculationVariable, manager);
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

        public override int GetCount(in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            return this.GetParentTablesAreSame(manager)
                ? this.GetFirst(manager).GetCount()
                : this.GetFirst(manager).GetCount() * this.GetSeccond(manager).GetCount();
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            var firstTable = this.GetFirst(manager);
            var seccondTable = this.GetSeccond(manager);
            return firstTable.GetVariables().Concat(seccondTable.GetVariables()).Concat(Enumerable.Repeat(this.ownP as IP, 1));
        }

        private bool GetParentTablesAreSame(in WhileManager manager)
        {
            var second = this.GetSeccond(manager);
            var first = this.GetFirst(manager);
            return first.table == second.table && Equals(first.manager, second.manager);
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager)}");

            int firstIndex;
            int secconedIndex;

            var firstTable = this.GetFirst(manager);
            var seccondTable = this.GetSeccond(manager);

            var sameTables = this.GetParentTablesAreSame(manager);
            if (sameTables)
            {
                firstIndex = index;
                secconedIndex = index;
            }
            else
            {
                firstIndex = index % firstTable.GetCount();
                secconedIndex = index / firstTable.GetCount();
            }

            if (p.Id == PropabilityKey.Id)
            {
                if (this.GetParentTablesAreSame(manager))
                    return firstTable.GetValue(p, index);
                return firstTable.GetValue(PropabilityKey, firstIndex) * seccondTable.GetValue(PropabilityKey, secconedIndex);
            }

            if (p.Id == this.ownP.Id)
            {
                var firstValue = firstTable.GetValue(this.FirstCalculationVariable, firstIndex);
                var seccondValue = seccondTable.GetValue(this.SeccondCalculationVariable, secconedIndex);
                return this.func(firstValue, seccondValue)!;
            }

            var inFirstTable = firstTable.Contains(p);
            var inSeccondTable = seccondTable.Contains(p);
            if (inFirstTable && (sameTables || !inSeccondTable))
                return firstTable.GetValue(p, firstIndex);
            if (inSeccondTable && (sameTables || !inFirstTable))
                return seccondTable.GetValue(p, secconedIndex);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);


            return key.Id == this.ownP.Id ? true : this.GetFirst(manager).Contains(key) || this.GetSeccond(manager).Contains(key);
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

        internal void Optimize(in WhileManager manager)
        {
            if (this.variablesToKeep != null)
            {
                // if one of both table has size one, we ommit optimizing sometimes this is good sometimes bad.
                if (this.OptimisationStrategy == OptimisationStrategy.NoOptimisation || (this.OptimisationStrategy == OptimisationStrategy.Guess && (this.GetFirst(manager).GetCount() == 1 || this.GetSeccond(manager).GetCount() == 1)))
                    return;

                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable);
            }

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager);
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

        public override int GetCount(in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            int count = 1;

            for (int i = 0; i < this.tables.Length; i++)
            {
                count *= this.GetTable(manager, i).GetCount();
            }
            return count;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Variables;

            var enumerable = Enumerable.Empty<IP>();
            for (int i = 0; i < this.tables.Length; i++)
                enumerable = enumerable.Concat(this.GetTable(manager, i).GetVariables());
            return enumerable;
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager)}");

            int[] tableIndex = new int[this.tables.Length];

            for (int i = 0; i < tableIndex.Length; i++)
            {
                var previousCount = 1;
                for (int j = 0; j < i; j++)
                    previousCount *= this.GetTable(manager, j).GetCount();

                tableIndex[i] = (index / previousCount) % this.GetTable(manager, i).GetCount();
            }


            if (p.Id == PropabilityKey.Id)
            {
                var propability = 1.0;
                for (int i = 0; i < tableIndex.Length; i++)
                    propability *= this.GetTable(manager, i).GetValue(PropabilityKey, tableIndex[i]);

                return propability;
            }

            for (int i = 0; i < tableIndex.Length; i++)
            {
                var table = this.GetTable(manager, i);
                if (table.Contains(p))
                    return table.GetValue(p, tableIndex[i]);
            }

            throw new KeyNotFoundException($"Key with id {p.Id}.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);

            for (int i = 0; i < this.tables.Length; i++)
                if (this.GetTable(manager, i).Contains(key))
                    return true;
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

        internal void Optimize(in WhileManager manager)
        {
            if (this.variablesToKeep != null)
                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable);

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager);
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

        public (WhileManager manager, Table table) GetTable(in WhileManager manager, int index)
        {
            return this.State.Parent.GetTable(this.variables[index], manager);
        }



        public CombinationTable(State state, P<T[]> combinationVariable, params P<T>[] variables) : base(state)
        {
            this.CombinationVariable = combinationVariable;
            this.variables = variables;
        }

        public override int GetCount(in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Count;

            int count = 1;

            var proxyManager = manager;
            var tables = this.variables.Select(x => (table: this.State.Parent.GetTable(x, proxyManager), variable: x)).GroupBy(x => x.table).ToArray();


            for (int i = 0; i < tables.Length; i++)
            {
                count *= tables[i].Key.GetCount();
            }
            return count;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Variables;

            return this.variables.Cast<IP>();
        }


        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable[p, index];


            if (index >= this.GetCount(manager))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager)}");

            var proxyManager = manager;

            var tables = this.variables.Select(x => (table: this.State.Parent.GetTable(x, proxyManager), variable: x)).GroupBy(x => x.table).ToArray();



            int[] tableIndex = new int[tables.Length];

            for (int i = 0; i < tableIndex.Length; i++)
            {
                var previousCount = 1;
                for (int j = 0; j < i; j++)
                    previousCount *= tables[j].Key.GetCount();

                tableIndex[i] = (index / previousCount) % tables[i].Key.GetCount();
            }

            if (p.Id == this.CombinationVariable.Id)
            {
                var result = new T[this.variables.Length];
                // combine all variables
                var counter = 0;
                for (int i = 0; i < tableIndex.Length; i++)
                {
                    var table = tables[i].Key;
                    foreach (var v in tables[i])
                    {
                        var vIndex = (this.variables as IList<P<T>>).IndexOf(v.variable);
                        result[vIndex] = table.GetValue(v.variable, tableIndex[i]);
                        counter++;
                    }


                }
                return result;
            }

            if (p.Id == PropabilityKey.Id)
            {
                var propability = 1.0;
                for (int i = 0; i < tableIndex.Length; i++)
                    propability *= tables[i].Key.GetValue(PropabilityKey, tableIndex[i]);

                return propability;
            }

            for (int i = 0; i < tableIndex.Length; i++)
            {
                var table = tables[i].Key;
                if (table.Contains(p))
                    return table.GetValue(p, tableIndex[i]);
            }

            throw new KeyNotFoundException($"Key with id {p.Id}.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            var cachedTable = this.CachedTable(manager);
            if (cachedTable != null)
                return cachedTable.Contains(key);

            if (key.Id == this.CombinationVariable.Id)
                return true;

            for (int i = 0; i < this.variables.Length; i++)
                if (this.GetTable(manager, i).Contains(key))
                    return true;
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

        internal void Optimize(in WhileManager manager)
        {
            if (this.variablesToKeep != null)
                this.cache.GetOrCreate(nameof(Optimize), manager, this.CreateOptimizedTable);

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            return new OptimizedTableCache(this, this.variablesToKeep, manager);
        }
    }

}
