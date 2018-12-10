using System;
using System.Collections.Generic;
using Dice.Caches;

namespace Dice.Tables
{
    internal class CombinationTable<TIn1, TIn2, TOut> : Table
    {
        private readonly Table First;
        private readonly Table Seccond;
        private readonly P<TOut> ownP;
        private readonly Func<TIn1, TIn2, TOut> func;
        private HashSet<IP>? variablesToKeep;
        private Cache? cache;

        public P<TIn1> FirstCalculationVariable { get; }
        public P<TIn2> SeccondCalculationVariable { get; }

        public CombinationTable(Table first, Table seccond, P<TOut> p, P<TIn1> firstCalculation, P<TIn2> seccondCalculation, Func<TIn1, TIn2, TOut> func)
        {
            this.First = first;
            this.Seccond = seccond;
            this.ownP = p;
            this.FirstCalculationVariable = firstCalculation;
            this.SeccondCalculationVariable = seccondCalculation;
            this.func = func;
        }

        public override int Count
        {
            get
            {

                if (this.cache != null)
                    return this.cache.Count;

                return this.ParentTablesAreSame
                    ? this.First.Count
                    : this.First.Count * this.Seccond.Count;
            }
        }


        private bool ParentTablesAreSame => this.First == this.Seccond;


        public override object GetValue(IP p, int index)
        {
            if (this.cache != null)
                return this.cache[p, index];


            if (index >= this.Count)
                throw new IndexOutOfRangeException($"The Index was out of Range index:{index} count:{this.Count}");

            int firstIndex;
            int secconedIndex;

            if (this.ParentTablesAreSame)
            {
                firstIndex = index;
                secconedIndex = index;
            }
            else
            {
                firstIndex = index % this.First.Count;
                secconedIndex = index / this.First.Count;
            }

            if (p.Id == PropabilityKey.Id)
            {
                if (this.ParentTablesAreSame)
                    return this.First.GetValue(p, index);
                return this.First.GetValue(PropabilityKey, firstIndex) * this.Seccond.GetValue(PropabilityKey, secconedIndex);
            }

            if (p.Id == this.ownP.Id)
            {
                var firstValue = this.First.GetValue(this.FirstCalculationVariable, firstIndex);
                var seccondValue = this.Seccond.GetValue(this.SeccondCalculationVariable, secconedIndex);
                return this.func(firstValue, seccondValue)!;
            }

            if (this.First.Contains(p))
                return this.First.GetValue(p, firstIndex);
            if (this.Seccond.Contains(p))
                return this.Seccond.GetValue(p, secconedIndex);

            throw new KeyNotFoundException($"Key with id {p.Id} of type {typeof(TIn1)} not found.");
        }

        protected override bool InternalContains(IP key) => key.Id == this.ownP.Id ? true : this.First.Contains(key) || this.Seccond.Contains(key);

        internal void Keep(IP nededVariable)
        {
            if (this.Contains(nededVariable))
            {
                this.variablesToKeep ??= new HashSet<IP>();
                this.variablesToKeep!.Add(nededVariable);
            }
        }

        internal void Optimize()
        {
            if (this.variablesToKeep != null)
                this.cache ??= new Cache(this, this.variablesToKeep);

        }
    }

}
