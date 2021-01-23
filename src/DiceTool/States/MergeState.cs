using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    class MergeState : State
    {
        private readonly Caches.WhilestateCache cache = new Caches.WhilestateCache();

        internal State Parent1 { get; }

        internal State Parent2 { get; }

        internal MergeTable? GetTable(in WhileManager manager)
        {
           return  this.cache.GetOrCreate(nameof(GetTable), manager, this.CreateTable);

            //if (this.cache.TryGet<MergeTable>(nameof(GetTable), manager, out var value))
            //    return value;
            //return null;
        }

        public MergeState(State parent1, State parent2) : base(null!)
        {
            this.Parent1 = parent1;
            this.Parent2 = parent2;
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            var prop1 = this.Parent1.GetStatePropability(manager);
            var prop2 = this.Parent2.GetStatePropability(manager);
            var propsum = prop1 + prop2;
            return propsum;
        }

        public override (WhileManager manager, Table table) GetTable(IP variable, in WhileManager manager)
        {
            var mergeTable = this.GetTable(manager);
            if (mergeTable is null)
                throw new InvalidOperationException("The Table must be optimzied before using this method");
            if (mergeTable.Contains(variable, manager))
                return (manager, mergeTable);


            if (Parent1.Contains(variable, manager))
                return this.Parent1.GetTable(variable, manager);
            return this.Parent2.GetTable(variable, manager);

        }



        internal override void Optimize(in WhileManager manager)
        {


            this.Parent1.Optimize(manager);
            this.Parent2.Optimize(manager);
            var mergeTable = this.GetTable(manager);
        }

        private MergeTable CreateTable(in WhileManager manager)
        {
            var variablesToCombine = new List<IP>();
            foreach (var x in this.NededVariables)
            {
                var table1 = this.Parent1.GetTable(x, manager);
                var table2 = this.Parent2.GetTable(x, manager);
                if (table1.table != null && table2.table != null && table1 != table2)
                {
                    variablesToCombine.Add(x);
                }
            }



            //this.NededVariables.;

            var toStore = new MergeTable(this, variablesToCombine, manager);
            return toStore;
            //return new Caches.OptimizedTableCache(toStore, variablesToCombine, manager);
        }

        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            base.PrepareOptimize(ps);
            this.Parent1.PrepareOptimize(ps);
            this.Parent2.PrepareOptimize(ps);
        }

        internal override StateEnumerable UpdateWhileManager(ref WhileManager manager)
        {
            return new StateEnumerable(this.Parent1, this.Parent2);
        }

        public override bool Contains(IP variable, in WhileManager manager)
        {
            return this.Parent1.Contains(variable, manager)
                || this.Parent2.Contains(variable, manager);
        }







    }
}
