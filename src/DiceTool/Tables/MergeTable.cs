using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Caches;
using Dice.States;

namespace Dice.Tables
{
    class MergeTable : Table
    {
        private readonly MergeState state;
        private readonly Table table1;
        private readonly Table table2;

        public MergeTable(MergeState state, IEnumerable<IP> variables, WhileManager manager) : base(state)
        {
            this.state = state;
            var multitable1 = variables
                //.Where(v => state.Contains(v, manager))
                .Select(v => (table: state.Parent1.GetTable(v, manager), variable: v))
                .GroupBy(x => x.table)
                .Select(x => (table: x.Key, variables: x.Select(y => y.variable)))
                .ToArray();
            var multitable2 = variables
                //.Where(v => state.Contains(v, manager))
                .Select(v => (table: state.Parent2.GetTable(v, manager), variable: v))
                .GroupBy(x => x.table)
                .Select(x => (table: x.Key, variables: x.Select(y => y.variable)))
                .ToArray();



            var table = new TableCombinationTable(this.state, multitable1.Select(x => x.table).ToArray());
            foreach (var item in multitable1.SelectMany(x => x.variables))
                table.Keep(item);
            table.Optimize(manager);
            this.table1 = table;

            var table2 = new TableCombinationTable(this.state, multitable2.Select(x => x.table).ToArray());
            foreach (var item in multitable1.SelectMany(x => x.variables))
                table2.Keep(item);
            table2.Optimize(manager);
            this.table2 = table2;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            return this.table1.GetVariables(manager);
        }


        public override int GetCount(in WhileManager manager)
        {
            return this.table1.GetCount(manager) + this.table2.GetCount(manager);
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            var table1Count = this.table1.GetCount(manager);
            if (p.Id != PropabilityKey.Id)
            {
                if (index < table1Count)
                    return this.table1.GetValue(p, index, manager);
                return this.table2.GetValue(p, index - table1Count, manager);
            }
            else
            {
                if (index < table1Count)
                    return (double)this.table1.GetValue(p, index, manager) * this.state.Parent1.GetStatePropability(manager);
                return (double)this.table2.GetValue(p, index - table1Count, manager) * this.state.Parent2.GetStatePropability(manager);
            }
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            return this.table1.Contains(key, manager);
        }
    }
}
