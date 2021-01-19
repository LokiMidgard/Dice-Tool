using System;
using System.Collections.Generic;
using System.Linq;
using Dice.States;

namespace Dice
{

    internal class StateAdministrator
    {
        private readonly State startState;
        internal State Current { get; private set; }

        public StateAdministrator(State startState)
        {
            this.startState = startState ?? throw new ArgumentNullException(nameof(startState));
            this.Current = startState;
        }


        public void NextStates(State state)
        {

            this.Current = state;
            //item.Children = new StateTree[] { current };
        }


        public IEnumerable<State> NextStates(params State[] stateFucntion) => this.NextStates(stateFucntion as IEnumerable<State>);
        public IEnumerable<State> NextStates(IEnumerable<State> stateFucntion)
        {

            var states = stateFucntion.ToArray();

            if (states.Length == 0)
                throw new ArgumentException("At least one state must be returned.");
            if (states.Length != 2)
                throw new NotImplementedException("Currently only exact twos states are supported.");


            var endStates = new List<State>();
            foreach (var currentState in states)
            {
                this.Current = currentState;
                yield return this.Current;
                endStates.Add(this.Current);
            }

            this.NextStates(new MergeState(endStates[0], endStates[1]));
        }



        //private StateTree current;

        //private IEnumerable<StateTree> GetLeaves()
        //{
        //    var queue = new Queue<StateTree>();
        //    queue.Enqueue(this.current);
        //    while (queue.Count > 0)
        //    {
        //        var s = queue.Dequeue();
        //        if (s.Children.Any())
        //            foreach (var item in s.Children)
        //                queue.Enqueue(item);
        //        else
        //            yield return s;
        //    }
        //}

        //public IEnumerable<State> CollectResults() => GetLeaves().Select(x => x.State);

        //private class StateTree
        //{
        //    public State State { get; set; }

        //    public StateTree Parent { get; set; }

        //    public IEnumerable<StateTree> Children { get; set; } = Enumerable.Empty<StateTree>();
        //}

    }

}
