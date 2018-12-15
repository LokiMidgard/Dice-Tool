using System.Linq;
using System.Collections.Generic;
using Dice.Tables;
using System;
using Dice.Caches;

namespace Dice.States
{
    internal class DevideState<T> : State
    {
        public T Value { get; }

        private readonly WhilestateCache cache;
        private readonly P<T> condition;

        internal DevideState(State parent, P<T> condition, T value) : base(parent)
        {
            this.condition = condition;
            //this.table = new DevideTable<T>(this, p, value);
            this.Value = value;
            this.cache = new Caches.WhilestateCache(this);
        }

        public override double GetStatePropability(in WhileManager manager)
        {
            double totoalPropability = 0.0;
            for (int i = 0; i < this.TableCount(manager); i++)
            {
                totoalPropability += this.GetCachedTable(i, manager).GetPartPropability(manager);
            }

            return base.GetStatePropability(manager) * totoalPropability;
        }

        public override (WhileManager manager, Table table) GetTable<T1>(P<T1> variable, int index, in WhileManager manager)
        {
            var parentTable = this.Parent.GetTable(this.condition, index, manager);
            if (parentTable.table.Contains(variable, manager))
            {
                return (manager, this.GetCachedTable(index, manager));
            }
            return base.GetTable(variable, index, manager);
        }

        private DevideTable<T> GetCachedTable(int index, in WhileManager manager)
        {
            if (this.cache.TryGet<DevideTable<T>>(index.ToString(), manager, out var cachedValue))
                return cachedValue;

            cachedValue = new DevideTable<T>(this, index, this.condition, this.Value);
            this.cache.Create(index.ToString(), manager, cachedValue);
            return cachedValue;
        }

        public override void PrepareOptimize(IEnumerable<IP> p, in WhileManager manager)
        {
            base.PrepareOptimize(p, manager);
        }

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return base.GetOptimizedVariablesForParent().Concat(new IP[] { this.condition });
        }
    }

}
