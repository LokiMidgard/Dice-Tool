using Dice.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dice.Tables
{
    internal class EmptyTable : Table
    {

        public override int GetCount(in WhileManager manager, CancellationToken cancellation)
        {
            return 0;
        }

        public EmptyTable(State state) : base(state)
        {
        }


        public override object GetValue(IP p, int index, in WhileManager manager, CancellationToken cancellation)
        {
            throw new KeyNotFoundException($"Key with id {p.Id}");
        }

        internal override IEnumerable<IP> GetVariables(in WhileManager manager, CancellationToken cancellation) => Enumerable.Empty<IP>();

        protected override bool InternalContains(IP key, in WhileManager manager, CancellationToken cancellation) => false;
    }

}
