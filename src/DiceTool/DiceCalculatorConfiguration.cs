using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dice
{
    public class DiceCalculatorConfiguration
    {
        /// <summary>
        /// Default Random
        /// </summary>
        public DiceResolution DiceResolution { get; set; } = DiceResolution.Random;


        /// <summary>
        /// If set provides the seed to the internal rng for deterministic results.
        /// </summary>
        public int? RandomSeed { get; set; }

        /// <summary>
        /// Specifies the number of Itterations (0 equals infinit).
        /// </summary>
        public ulong NumberOfIterrations { get; set; }
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
