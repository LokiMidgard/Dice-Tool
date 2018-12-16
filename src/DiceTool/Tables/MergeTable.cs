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
        private readonly State state;
        private readonly Table table;

        public MergeTable(State state, IEnumerable<IP> variables, WhileManager manager)
        {
            this.state = state;
            var multitable = variables
                .Where(v => state.Contains(v, manager))
                .Select(v => (table: state.GetTable(v, manager), variable: v))
                .GroupBy(x => x.table)
                .Select(x => (table: x.Key, variables: x.Select(y => y.variable)))
                .ToArray();

            if (multitable.Length > 1)
            {

                var cachedTable = multitable
                    .Aggregate((first, secconed) =>
                    {
                        var table = new CombinationTable(first.table, secconed.table);
                        var combinedVariables = first.variables.Concat(secconed.variables);
                        foreach (var item in combinedVariables)
                            table.Keep(item);
                        table.Optimize(manager);

                        return ((manager, table), combinedVariables);
                    });

                this.table = cachedTable.table.table;
            }
            else
            {
                var table = new CombinationTable(multitable[0].table, multitable[0].table);
                foreach (var item in multitable[0].variables)
                    table.Keep(item);
                table.Optimize(manager);
                this.table = table;

            }

            //    .Select(x =>
            //    {

            //        var c = new Caches.OptimizedTableCache(x.Key.table, x.Select(x1 => x1.variable), manager);

            //        return c;
            //    });

            //this.OptimizedTables = new Dictionary<IP, Caches.OptimizedTableCache>();
            //foreach (var table in groupedTables.SelectMany(x => x.Variables.Select(y => (table: x, variable: y))))
            //    this.OptimizedTables.Add(table.variable, table.table);


        }

        public override int GetCount(in WhileManager manager)
        {
            return table.GetCount(manager);
        }

        public override object GetValue(IP p, int index, in WhileManager manager)
        {
            return table.GetValue(p, index, manager);
        }

        protected override bool InternalContains(IP key, in WhileManager manager)
        {
            return table.Contains(key, manager);
        }
    }
}
