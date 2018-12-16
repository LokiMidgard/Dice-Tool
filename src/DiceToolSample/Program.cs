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
                var dice = x.Dice(6);
                var @const = x.Const(5);
                var result = dice.GreaterOrEqual(@const);
                return result;
            });

            var simpleIf = Calculator<int>.Configure(x =>
            {
                x.AssignName("result", x.Const(true));
                x.If(x.Dice(6).LessOrEqual(x.Const(3)), () => x.AssignName("result", x.Const(false)));
                return x.GetNamed<bool>("result");
            });

            var doubleIf = Calculator<int>.Configure(x =>
            {
                x.AssignName("result", x.Const(0));
                x.If(x.Dice(6).LessOrEqual(x.Const(3)), () => x.AssignName("result", x.Const(-1)));
                x.If(x.Dice(6).LessOrEqual(x.Const(3)), () => x.AssignName("result", x.Const(+1)));
                return x.GetNamed<int>("result");
            });

            var simpleWhile = Calculator<int>.Configure(x =>
            {
                x.AssignName("result", x.Const(0));
                x.DoWhile(() =>
                {
                    x.AssignName("result", x.GetNamed<int>("result").Add(x.Const(1)));
                    return x.Dice(2).AreEqual(x.Const(1));
                });
                return x.GetNamed<int>("result");
            });

            var simpleWhile2 = Calculator<int>.Configure(x =>
            {
                x.AssignName("y", x.Const(0));
                const int Faces = 4;
                x.AssignName("x", x.Dice(Faces));
                x.DoWhile(() =>
                {
                    x.AssignName("x", x.GetNamed<int>("x").Add(x.Const(1)));
                    x.If(x.GetNamed<int>("x").Modulo(x.Const(2)).AreEqual(x.Const(0)), () =>
                    {
                        x.AssignName("y", x.GetNamed<int>("y").Add(x.Const(1)));
                    });
                    return x.GetNamed<int>("x").LessThen(x.Const(Faces + 1));
                });
                return x.GetNamed<int>("y");
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
                x.AssignName("taw", x.Const(10)); // Skill
                x.AssignName("e1", x.Const(13)); // Attribute 1
                x.AssignName("e2", x.Const(13)); // Attribute 2
                x.AssignName("e3", x.Const(13)); // Attribute 3
                x.AssignName("w1", x.Dice(20));
                x.AssignName("w2", x.Dice(20));
                x.AssignName("w3", x.Dice(20));

                // if dificulty is higher then our skill level, then we substract the negative value from every atribute
                x.AssignName("taw", x.GetNamed<int>("taw").Substract(dificulty));
                x.If(x.GetNamed<int>("taw").LessThen(x.Const(0)),
                        then: () =>
                        {
                            x.AssignName("e1", x.GetNamed<int>("e1").Add(x.GetNamed<int>("taw")));
                            x.AssignName("e2", x.GetNamed<int>("e2").Add(x.GetNamed<int>("taw")));
                            x.AssignName("e3", x.GetNamed<int>("e3").Add(x.GetNamed<int>("taw")));
                            x.AssignName("taw", x.Const(0));
                        });

                x.If(x.GetNamed<int>("w1").GreaterThen(x.GetNamed<int>("e1")),
                    then: () =>
                    {
                        x.AssignName("taw", x.GetNamed<int>("taw").Substract(x.GetNamed<int>("w1").Substract(x.GetNamed<int>("e1"))));
                    });

                x.If(x.GetNamed<int>("w2").GreaterThen(x.GetNamed<int>("e2")),
                    then: () =>
                    {
                        x.AssignName("taw", x.GetNamed<int>("taw").Substract(x.GetNamed<int>("w2").Substract(x.GetNamed<int>("e2"))));
                    });

                x.If(x.GetNamed<int>("w3").GreaterThen(x.GetNamed<int>("e3")),
                    then: () =>
                    {
                        x.AssignName("taw", x.GetNamed<int>("taw").Substract(x.GetNamed<int>("w3").Substract(x.GetNamed<int>("e3"))));
                    });

                x.If(x.GetNamed<int>("taw").LessThen(x.Const(0)),
                    then: () =>
                    {
                        x.AssignName("taw", x.Const(-1));
                    });

                x.If(x.GetNamed<int>("taw").AreEqual(x.Const(0)),
                    then: () =>
                    {
                        x.AssignName("taw", x.Const(1));
                    });

                return x.GetNamed<int>("taw");
            });

            Console.WriteLine($"Configuration took {watch.Elapsed}");

            //Console.WriteLine("simple Rolle");
            //PrintResults(simpleDiceRole);

            //Console.WriteLine("simple if");
            //PrintResults(simpleIf);

            //Console.WriteLine("double if");
            //PrintResults(doubleIf);

            //Console.WriteLine("simple while");
            //PrintResults(simpleWhile);

            Console.WriteLine("simple while 2");
            PrintResults(simpleWhile2);

            //Console.WriteLine("multi Dice roll");
            //PrintResults(multiDiceRoll);

            //Console.WriteLine("Masive role");
            //PrintResults(massiveRole);

            //Console.WriteLine("The Dark Eye role");
            //PrintResults(theDarkEyeRole);


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
            //Console.WriteLine($"GetEnumerator Took {watch.Elapsed}");
            watch.Restart();

            while (enumerator.MoveNextAsync().Result)
            {
                var item = enumerator.Current;
                Console.WriteLine($"{item.Result}:\t{item.Propability * 100:00.00}%");
                sum += item.Propability;

                //Console.WriteLine($"GetElement took {watch.Elapsed}");
                watch.Restart();
            }
            enumerator.DisposeAsync().AsTask().Wait();
            Console.WriteLine($"Sum {sum}");
            Console.ReadKey(true);
        }
    }
}
