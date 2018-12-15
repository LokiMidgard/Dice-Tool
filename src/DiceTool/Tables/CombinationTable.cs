using System;
using System.Collections.Generic;
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
        private Cache? cache;

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

            if (this.cache != null)
                return this.cache.Count;

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
            if (this.cache != null)
                return this.cache[p, index];


            if (index >= this.GetCount(manager))
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.GetCount(manager)}");

            int firstIndex;
            int secconedIndex;

            if (this.GetParentTablesAreSame(manager))
            {
                firstIndex = index;
                secconedIndex = index;
            }
            else
            {
                firstIndex = index % this.GetFirst(manager).GetCount();
                secconedIndex = index / this.GetFirst(manager).GetCount();
            }

            if (p.Id == PropabilityKey.Id)
            {
                if (this.GetParentTablesAreSame(manager))
                    return this.GetFirst(manager).GetValue(p, index);
                return this.GetFirst(manager).GetValue(PropabilityKey, firstIndex) * this.GetSeccond(manager).GetValue(PropabilityKey, secconedIndex);
            }

            if (p.Id == this.ownP.Id)
            {
                var firstValue = this.GetFirst(manager).GetValue(this.FirstCalculationVariable, firstIndex);
                var seccondValue = this.GetSeccond(manager).GetValue(this.SeccondCalculationVariable, secconedIndex);
                return this.func(firstValue, seccondValue)!;
            }

            if (this.GetFirst(manager).Contains(p))
                return this.GetFirst(manager).GetValue(p, firstIndex);
            if (this.GetSeccond(manager).Contains(p))
                return this.GetSeccond(manager).GetValue(p, secconedIndex);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key, in WhileManager manager) => key.Id == this.ownP.Id ? true : this.GetFirst(manager).Contains(key) || this.GetSeccond(manager).Contains(key);

        internal void Keep(IP nededVariable, in WhileManager manager)
        {
            if (this.Contains(nededVariable, manager))
            {
                this.variablesToKeep ??= new HashSet<IP>();
                this.variablesToKeep!.Add(nededVariable);
            }
        }

        internal void Optimize(in WhileManager manager)
        {
            if (this.variablesToKeep != null)
                this.cache ??= new Cache(this, this.variablesToKeep, manager);

        }
    }

}
