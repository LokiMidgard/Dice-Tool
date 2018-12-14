using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Dice.Tables;

namespace Dice.States
{
    class MergeState : State
    {
        private readonly State[] Parents;
        private readonly Dictionary<IP, MergeTable> tables = new Dictionary<IP, MergeTable>();

        public MergeState(IEnumerable<State> parents) : base(null!)
        {
            this.Parents = parents.ToArray();
        }

        public override Table GetTable<T>(P<T> index)
        {
            if (!this.tables.ContainsKey(index))
            {
                var tables = this.Parents.Select(x => x.GetTable(index));
                this.tables.Add(index, new MergeTable(tables));
            }
            return this.tables[index];
        }

        public override int Depth => this.Parents.Max(x => x.Depth);

        protected internal override IEnumerable<IP> GetOptimizedVariablesForParent()
        {
            return this.Parents.SelectMany(x => x.GetOptimizedVariablesForParent());
        }


        public override void PrepareOptimize(IEnumerable<IP> ps)
        {
            foreach (var p in this.Parents)
                p.PrepareOptimize(ps);
        }

        internal override void Optimize()
        {
            foreach (var p in this.Parents)
                p.Optimize();
        }

        public override double StatePropability => this.Parents.Sum(x => x.StatePropability);

    }
}
