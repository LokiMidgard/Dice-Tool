using System;
using Dice.States;

namespace Dice
{

    internal class StateAdministrator
    {
        private readonly State startState;

        public State CurrentState { get; private set; }

        private LastChoise? lastPath;
        private LastChoise? currentPath;

        public StateAdministrator(State startState)
        {
            this.startState = startState ?? throw new ArgumentNullException(nameof(startState));
            this.CurrentState = startState;
        }

        public bool MoveNext()
        {
            this.lastPath = this.currentPath;
            while (this.lastPath != null && this.lastPath?.choise == this.lastPath?.maxChoises - 1)
                this.lastPath = this.lastPath.parent; // remove completed pathes

            this.currentPath = null;
            this.CurrentState = this.startState;

            return this.lastPath != null; // we have no last path, either we never encuntered a fork, or we have calculated the last posible way.

        }

        public ISaveState ExportCurrentState()
        {
            return new SaveState(this.lastPath);
        }

        public void ImportSavedState(ISaveState state)
        {
            if (state is SaveState saveState)
                this.lastPath = saveState.LastPath;
            else
                throw new ArgumentException("Save state was not generated from this Class.");
        }


        public State NextStates(params State[] states)
        {
            State stateToReturn;
            if (states.Length == 1)
            {
                stateToReturn = states[0];
            }
            else
            {
                var numberOfChoises = states.Length;

                if (this.lastPath != null)
                {

                    if (this.lastPath.depth == this.CurrentState.Depth)
                    {
                        // we are at position when last time we did the last decision
                        // so now we need to do another decision.
                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = this.lastPath.choise + 1,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };
                    }
                    else if (this.lastPath.depth > this.CurrentState.Depth)
                    {
                        // We need to follow the last path further
                        var current = this.lastPath;
                        while (current.depth != this.CurrentState.Depth)
                            current = current.parent ?? throw new InvalidOperationException("Did not find correct parent");

                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = current.choise,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };
                    }
                    else
                    {
                        // this is the first time we got here
                        this.currentPath = new LastChoise()
                        {
                            depth = this.CurrentState.Depth,
                            choise = 0,
                            maxChoises = numberOfChoises,
                            parent = currentPath
                        };

                    }
                    stateToReturn = states[this.currentPath.choise];
                }
                else
                {
                    // this is the first time so we will always take the first choise
                    this.currentPath = new LastChoise()
                    {
                        depth = this.CurrentState.Depth,
                        choise = 0,
                        maxChoises = numberOfChoises,
                        parent = currentPath
                    };
                    stateToReturn = states[0];
                }



            }

            return this.CurrentState = stateToReturn;
        }


        private class LastChoise
        {
            public int depth;
            public int choise;
            public int maxChoises;
            public LastChoise? parent;
        }

        private class SaveState : ISaveState
        {
            public readonly LastChoise? LastPath;

            public SaveState(LastChoise? lastPath)
            {
                this.LastPath = lastPath;
            }
        }

        public interface ISaveState
        {
        }
    }

}
