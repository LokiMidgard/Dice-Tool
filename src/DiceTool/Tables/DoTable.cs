using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dice.Caches;
using Dice.States;

namespace Dice.Tables
{
    class DoTable : Table
    {
        private readonly State state;
        private readonly Table table;

        public DoTable(State state, IEnumerable<IP> variables, WhileManager manager) : base(state)
        {
            this.state = state;
            var multitable = variables
                //.Where(v => state.Contains(v, manager))
                .Select(v => (table: state.GetTable(v, manager), variable: v))
                .GroupBy(x => x.table)
                .Select(x => (table: x.Key, variables: x.Select(y => y.variable)))
                .ToArray();



            var table = new TableCombinationTable(this.state, multitable.Select(x => x.table).ToArray());
            foreach (var item in multitable.SelectMany(x => x.variables))
                table.Keep(item);
            table.Optimize(manager);
            this.table = table;
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager)
        {
            return this.table.GetVariables(manager);
        }


        public override int GetCount(in WhileManager manager)
        {
            return this.table.GetCount(manager);
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            return this.table.GetValue(p, index, manager);
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            return this.table.Contains(key, manager);
        }
    }
}
