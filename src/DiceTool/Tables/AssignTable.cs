using System;
using System.Collections.Generic;
using System.Text;

namespace Dice.Tables
{
    class AssignTable<T> : Table
    {
        private readonly P<T> variable;
        private readonly P<T> newContent;
        private readonly Table originalTable;

        public AssignTable(P<T> variable, P<T> newContent, Table originalTable)
        {
            this.variable = variable;
            this.newContent = newContent;
            this.originalTable = originalTable;
        }

        public override int Count => this.originalTable.Count;


        public override object GetValue(IP p, int index)
        {
            if (p.Id == this.variable.Id)
                return this.originalTable.GetValue(this.newContent as IP, index);
            return this.originalTable.GetValue(p, index);
        }

        protected override bool InternalContains(IP key)
        {
            return key.Id == this.variable.Id || this.originalTable.Contains(key);
        }

    }
}
