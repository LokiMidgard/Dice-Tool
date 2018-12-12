using System;
using System.Collections.Generic;
using System.Text;

namespace Dice.Tables
{
    internal class MaskTable : Table
    {
        public MaskTable(IEnumerable<(IP substitue, IP original, Table originalTable)> mappings)
        {
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
