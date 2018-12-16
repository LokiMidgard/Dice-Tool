using Dice.States;
using System;

namespace Dice
{
    internal class InitialWhileManager : IWhileManager
    {

        public InitialWhileManager(int whileIndex, int whileCount)
        {
            this.counter = new int[whileCount];
            Array.Fill(counter, whileIndex);
        }

        private int[] counter;

        public int Count => counter.Length;

        public (int count, WhileState state) this[int index] => (this.counter[index], null!);
    }
}