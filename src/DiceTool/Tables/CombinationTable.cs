using System;
using System.Collections.Generic;
using System.Linq;
using Dice.Caches;
using Dice.States;

namespace Dice.Tables
{
    internal class CombinationTable<TIn1, TIn2, TOut> : Table
    {
        private readonly CombinationState<TIn1, TIn2, TOut> state;
        private readonly P<TOut> ownP;
        private readonly Func<TIn1, TIn2, TOut> func;
        private HashSet<IP>? variablesToKeep;
        //private OptimizedTableCache? cache;
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

        public CombinationTable(CombinationState<TIn1, TIn2, TOut> parent, P<TOut> p, P<TIn1> firstCalculation, P<TIn2> seccondCalculation, Func<TIn1, TIn2, TOut> func)
        {
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

            if (this.GetParentTablesAreSame(manager))
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

            if (firstTable.Contains(p))
                return firstTable.GetValue(p, firstIndex);
            if (seccondTable.Contains(p))
                return seccondTable.GetValue(p, secconedIndex);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => key.Id == this.ownP.Id ? true : this.GetFirst(manager).Contains(key) || this.GetSeccond(manager).Contains(key);

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
            //this.cache ??= new OptimizedTableCache(this, this.variablesToKeep, manager);

        }

        private OptimizedTableCache CreateOptimizedTable(in WhileManager manager)
        {
            System.Diagnostics.Debug.Assert(this.variablesToKeep != null);
            var list = new List<IP>();
            foreach (var item in variablesToKeep)
            {
                //if (state.Contains(item, manager))
                if (this.Contains(item, manager))
                    list.Add(item);
            }
            return new OptimizedTableCache(this, list, manager);
        }
    }

}
