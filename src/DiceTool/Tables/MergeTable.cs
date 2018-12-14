using System;
using System.Collections.Generic;
using System.Text;

namespace Dice.Tables
{
    internal class MergeTable : Table
    {
        private IEnumerable<Table> tables;

        public MergeTable(IEnumerable<Table> tables)
        {
            this.tables = tables;
        }

        public override int Count => throw new NotImplementedException();

        public override object GetValue(IP p, int index)
        {
            throw new NotImplementedException();
        }

        protected override bool InternalContains(IP key)
        {
            throw new NotImplementedException();
        }
    }
}
