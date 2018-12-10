using Dice;
using System;


namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var simpleDiceRole = Calculator<int>.Configure(x =>
            {
                return x.Dice(6).GreaterOrEqual(x.Const(5));
            });

            var multiDiceRoll = Calculator<int>.Configure(x =>
            {
                return x.Dice(6)
                    .Add(x.Dice(6))
                    .Add(x.Dice(6));
            });

            var massiveRole = Calculator<int>.Configure(x =>
            {

                var adition = x.Dice(120)
                    .Add(x.Dice(120))
                    .Add(x.Dice(120))
                    .Add(x.Dice(120));
                return adition.GreaterThen(x.Const(61 + 61 + 61 + 61));

            });

            var theDarkEyeRole = Calculator<int>.Configure(x =>
            {
                var dificulty = x.Const(3); // dificulty
                var taw = x.Const(10); // Skill
                var e1 = x.Const(13); // Attribute 1
                var e2 = x.Const(13); // Attribute 2
                var e3 = x.Const(13); // Attribute 3
                var w1 = x.Dice(20);
                var w2 = x.Dice(20);
                var w3 = x.Dice(20);

                // if dificulty is higher then our skill level, then we substract the negative value from every atribute
                taw = taw.Substract(dificulty);
                taw.LessThen(x.Const(0)).Then(() =>
                {
                    e1 = e1.Add(taw);
                    e2 = e2.Add(taw);
                    e3 = e3.Add(taw);
                    taw = x.Const(0);
                });

                w1.GreaterThen(e1).Then(() =>
                {
                    taw = taw.Substract(w1.Substract(e1));
                });

                w2.GreaterThen(e2).Then(() =>
                {
                    taw = taw.Substract(w2.Substract(e2));
                });

                w3.GreaterThen(e3).Then(() =>
                {
                    taw = taw.Substract(w3.Substract(e3));
                });

                taw.LessThen(x.Const(0)).Then(() =>
                {
                    taw = x.Const(-1);
                });

                taw.AreEqual(x.Const(0)).Then(() =>
                {
                    taw = x.Const(1);
                });

                return taw;
            });

            Console.WriteLine($"Configuration took {watch.Elapsed}");

            Console.WriteLine("simple Rolle");
            PrintResults(simpleDiceRole);

            Console.WriteLine("multi Dice roll");
            PrintResults(multiDiceRoll);

            Console.WriteLine("Masive role");
            PrintResults(massiveRole);

            Console.WriteLine("The Dark Eye role");
            PrintResults(theDarkEyeRole);


            //await foreach (var item in asyncEnumerable)
            //{
            //    Console.WriteLine($"{item.Result}:\t{(item.Propability * 100):00.00}%");
            //    sum += item.Propability;
            //}
            Console.WriteLine("FINISHED");
            Console.ReadKey(true);
        }

        private static void PrintResults<T>(IExecutor<T, int> executor)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var asyncEnumerable = executor.Calculate(2);
            double sum = 0;

            var enumerator = asyncEnumerable.GetAsyncEnumerator();
            Console.WriteLine($"GetEnumerator Took {watch.Elapsed}");
            watch.Restart();

            while (enumerator.MoveNextAsync().Result)
            {
                var item = enumerator.Current;
                Console.WriteLine($"{item.Result}:\t{item.Propability * 100:00.00}%");
                sum += item.Propability;

                Console.WriteLine($"GetElement took {watch.Elapsed}");
                watch.Restart();
            }
            enumerator.DisposeAsync().AsTask().Wait();
            Console.WriteLine($"Sum {sum}");
            Console.ReadKey(true);
        }
    }
}
