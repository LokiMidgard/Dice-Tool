﻿using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dice.States
{
    internal class NewVariableState<T> : TableState<SingelVariableTable<T>>
    {

        private readonly SingelVariableTable<T> table;
        private readonly P<T> variable;

        public NewVariableState(State parent, P<T> variable, params (T value, double propability)[] distribution) : base(parent)
        {
            this.table = new SingelVariableTable<T>(this, variable, distribution);
            this.variable = variable;
        }

        public override SingelVariableTable<T> Table => this.table;

        protected internal override IEnumerable<IP> GetVarialesProvidedByThisState(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            return base.GetVarialesProvidedByThisState(cancellation).Concat(new IP[] { this.variable });
        }

    }

}
