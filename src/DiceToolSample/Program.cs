using Dice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceToolSample
{
    class Program
    {
        static void Main(string[] args)
        {
            b1().Wait();
            Console.ReadKey(true);
            Console.Clear();
            b2().Wait();
            Console.ReadKey(true);
            Console.Clear();
            b3().Wait();
            Console.ReadKey(true);
            Console.Clear();
            b4().Wait();
            Console.ReadKey(true);
            Console.Clear();
            return;
        }

        public static async Task b1()
        {
            var simpleDiceGenerator = new SimpleDiceRole();
            var results = await simpleDiceGenerator.DoIt();
            foreach (var r in results)
                Console.WriteLine($"{r.Result}: {(r.Propability * 100):f2}%");

        }

        public static async Task b2()
        {
            var repeatingDiceGenerator = new RepeatingDiceRole();
            var tokenSource = new System.Threading.CancellationTokenSource();
            tokenSource.CancelAfter(500);
            var results = await repeatingDiceGenerator.DoIt(tokenSource.Token);
            foreach (var r in results.OrderBy(x => x.Result).Take(19))
                Console.WriteLine($"{r.Result}: {(r.Propability * 100):f2}%");
        }

        public static async Task b3()
        {
            var multiDiceGenerator = new MultiDiceRole();
            var results = await multiDiceGenerator.DoIt(new int[] { 1, 2, 3 });
            foreach (var f in results.GroupBy(x => x.Item1).OrderBy(x => x.Key))
            {
                Console.WriteLine($"# of dice:{f.Key}");
                foreach (var r in f.OrderBy(x => x.Result))
                    Console.WriteLine($"\t{r.Result}: {(r.Propability * 100):f2}%");
            }

        }

        public static async Task b4()
        {
            var theDarkEyeGenerator = new ThDarkEyeRole();
            var calculateTotalPosibilitys = 8 * 8 * 8 * 15 * 11 * 20 * 20 * 20;
            var results = await theDarkEyeGenerator.DoIt(
                Enumerable.Range(7, 8).ToList(),    // Attribute 1
                Enumerable.Range(7, 8).ToList(),    // Attribute 2
                Enumerable.Range(7, 8).ToList(),    // Attribute 3
                Enumerable.Range(0, 15).ToList(),   // Skill
                Enumerable.Range(-3, 11).ToList(),  // Dificulty
                new Progress<int>(status =>
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Console.WriteLine($"{status}/{calculateTotalPosibilitys}");
                    Console.Write("[");
                    const int progressbarWidth = 50;
                    for (int i = 0; i < progressbarWidth; i++)
                    {
                        if (i < status * progressbarWidth / (long)calculateTotalPosibilitys)
                            Console.Write("=");
                        else
                            Console.Write("-");

                    }
                    Console.Write("]");

                }),
                TimeSpan.FromSeconds(0.5),
                null,
                default(TimeSpan),
                configuration: new DiceCalculatorConfiguration()
                {
                    DiceResolution = DiceResolution.SmallestDiceFirst
                });
            foreach (var f in results.GroupBy(x => x.Item1).OrderBy(x => x.Key))
            {
                Console.WriteLine($"# of dice:{f.Key}");
                foreach (var r in f.OrderBy(x => x.Result))
                    Console.WriteLine($"\t{r.Result}: {(r.Propability * 100):f2}%");
            }

        }
    }


    class ThDarkEyeRole : Dice.DiceCalculator<int, int, int, int, int, int>
    {

        protected override int RoleCalculation(int attribute1, int attribute2, int attribute3, int skillValue, int dificulty)
        {
            int effectiveSkill = skillValue - dificulty;

            // if it its more difficult than our skill substract from every Attribute
            if (effectiveSkill < 0)
            {
                attribute1 += effectiveSkill;
                attribute2 += effectiveSkill;
                attribute3 += effectiveSkill;
                effectiveSkill = 0;
            }

            int r1 = D20;
            int r2 = D20;
            int r3 = D20;

            // calculate what we've lost when we rolled for
            // every Attribute. We must role equal or less then
            // the value.

            int miss1 = Math.Min(0, attribute1 - r1);
            int miss2 = Math.Min(0, attribute2 - r2);
            int miss3 = Math.Min(0, attribute3 - r3);

            var totalMiss = miss1 + miss2 + miss3;

            // We return what we had left of points. But not more then
            // our initial Skill value. And -1 if we failed.
            var result = Math.Max(-1, Math.Min(effectiveSkill - totalMiss, skillValue));
            return result;
        }
    }

    class SimpleDiceRole : Dice.DiceCalculator<bool>
    {
        protected override bool RoleCalculation()
        {
            return D6 >= 5;
        }
    }
    class RepeatingDiceRole : Dice.DiceCalculator<int>
    {
        protected override int RoleCalculation()
        {
            int result = 0;
            int role;
            do
            {
                role = D6;
                result += role;
            } while (role == 3);
            return result;
        }
    }





    class MultiDiceRole : Dice.DiceCalculator<int, int>
    {
        protected override int RoleCalculation(int numberOfDices)
        {
            return numberOfDices * W6;
        }
    }

}
