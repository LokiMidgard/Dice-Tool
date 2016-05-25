using Dice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceToolSample
{
    class Program
    {
        private static readonly Random r = new Random();

        static void Main(string[] args)
        {
            int counter = 0;
            var busyIndicator = new char[] { '-', '\\', '|', '/' };
            var t = new TestImpl();
            var task = t.DoIt(Enumerable.Range(8, 1).ToArray(), Enumerable.Range(8, 1).ToArray(), Enumerable.Range(8, 1).ToArray(), Enumerable.Range(0, 21).ToArray(), Enumerable.Range(0, 1).ToArray(), new Progress<int>(progres =>
            {
                //Console.CursorLeft = 0;
                //Console.CursorTop = 0;
                Console.WriteLine($"{busyIndicator[unchecked(counter++) % busyIndicator.Length]}  Bisher {progres,0:n} berechnet.            ");
                //Console.WriteLine($"Geschätzt \n{progres.Max(x=>x.Factor),0:n}                    ");
            }), TimeSpan.FromSeconds(2), null, default(TimeSpan));



            var erg = task.Result;

            foreach (var item in erg.Where(x => x.Result).Where(x => x.Item1 == 8 && x.Item2 == 8 && x.Item3 == 8).Where(x => x.Item5 == 0).GroupBy(x => x.Item4).OrderBy(x => x.Key).Select(x => new { Percentage = x.Sum(y => y.Propability), Value = x.Key }))
            {
                Console.WriteLine($"{item.Value}: {100.0 * item.Percentage,2:F}%");
                //Console.WriteLine($"{item.Key}: {100.0 * item.Value/ erg.Sum(y => y.Value),2:F}% ({item.Value}/{erg.Sum(y => y.Value)})");
                //Console.WriteLine($"{item.Key}: {100.0 * agreatedValue / erg.Sum(y => y.Value),2:F}% ({agreatedValue}/{erg.Sum(y => y.Value)})");
            }


            Console.ReadKey(true);
        }


        public class TestImpl : DiceCalculator<bool, int, int, int, int, int>
        {
            protected override bool RoleCalculation(int e1, int e2, int e3, int taw, int dificulty)
            {

                //int i = 2 * W6;
                //return i == taw;

                int d1 = W20;
                int d2 = W20;
                int d3 = W20;

                taw -= dificulty;

                if (taw < 0)
                {
                    e1 += taw;
                    e2 += taw;
                    e3 += taw;
                    taw = 0;
                }

                taw -= (Math.Max(0, d1 - e1) + Math.Max(0, d2 - e2) + Math.Max(0, d3 - e3));
                return taw >= 0;
            }


        }
    }
}
