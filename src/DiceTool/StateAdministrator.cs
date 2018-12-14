using System;
using System.Collections.Generic;
using System.Linq;
using Dice.States;

namespace Dice
{

    internal class StateAdministrator
    {
        private readonly State startState;


        public StateAdministrator(State startState)
        {
            this.startState = startState ?? throw new ArgumentNullException(nameof(startState));
            this.current = new StateTree() { State = startState };
        }

        public void NextStates(Func<State, State> state)
        {
            foreach (var item in this.GetLeaves())
            {
                var s = state(item.State);
                var current = new StateTree() { Parent = item, State = s };
                item.Children = new StateTree[] { current };
            }
        }

        public IEnumerable<State> NextStates<T>(Func<State, IEnumerable<DevideState<T>>> stateFucntion)
        {

            foreach (var item in this.GetLeaves())
            {
                var states = stateFucntion(item.State).ToArray();

                if (states.Length == 0)
                    throw new ArgumentException("At least one state must be returned.");

                var children = states.Select(x => new StateTree() { State = x, Parent = item }).ToArray();
                item.Children = children;
                var oldCurrent = this.current;
                foreach (var currentState in children)
                {
                    this.current = currentState;
                    yield return this.current.State;
                }
                this.current = oldCurrent;

            }
        }



        private StateTree current;

        private IEnumerable<StateTree> GetLeaves()
        {
            var queue = new Queue<StateTree>();
            queue.Enqueue(this.current);
            while (queue.Count > 0)
            {
                var s = queue.Dequeue();
                if (s.Children.Any())
                    foreach (var item in s.Children)
                        queue.Enqueue(item);
                else
                    yield return s;
            }
        }

        public IEnumerable<State> CollectResults() => GetLeaves().Select(x => x.State);

        private class StateTree
        {
            public State State { get; set; }

            public StateTree Parent { get; set; }

            public IEnumerable<StateTree> Children { get; set; } = Enumerable.Empty<StateTree>();
        }

    }

}
