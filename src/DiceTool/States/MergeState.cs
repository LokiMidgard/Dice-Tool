using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dice.States
{
    class MergeState : State
    {
        public MergeState(IEnumerable<State> parents) : base(parents.First())
        {
        }


        public void AddMapping(State parent, IP from, IP to)
        {

        }
    }
}
