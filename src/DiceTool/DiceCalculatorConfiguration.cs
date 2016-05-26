using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dice
{
    public class DiceCalculatorConfiguration
    {
        public DiceResolution DiceResolution { get; set; } = DiceResolution.Random;
    }

    public enum DiceResolution
    {
        /// <summary>
        /// Randomly rolls the Dices. Best if you use exploding dices. But the memory consumption is higher. If you run out of Memory, use one of the other values.
        /// </summary>
        Random,
        /// <summary>
        /// first rolls the smallest numbers. Can lead to wrongly calculated Provability's if used with exploding dices. But have a lower memory Footprint then Random.
        /// </summary>
        SmallestDiceFirst,
        /// <summary>
        /// first rolls the highest numbers. Can lead to wrongly calculated Provability's if used with exploding dices. But have a lower memory Footprint then Random.
        /// </summary>
        LargestDiceFirst
    }
}
