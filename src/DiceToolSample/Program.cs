using Dice;
using ExcelDataReader;
using System;
using System.IO;

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
                const int Faces = 6;
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

            var simpleWhile3 = Calculator<int>.Configure(x =>
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

            var notaMiasma = Calculator<int>.Configure(x =>
            {
                var table = T();
                const string soule = "soule";
                const string counter = "counter";
                x.AssignName(counter, x.Const(0));
                x.AssignName(soule, x.Const(100));

                x.DoWhile(() =>
                {
                    const string coruption = "coruption";
                    const string value = "value";
                    const string role = "role";
                    x.AssignName(counter, x.GetNamed<int>(counter).Add(x.Const(1)));
                    x.AssignName(coruption, x.Const(100).Substract(x.GetNamed<int>(soule)));
                    x.AssignName(value, x.Const(150).Add(x.Const(2).Multiply(x.GetNamed<int>(coruption))).Substract(x.GetNamed<int>(soule)));

                    x.AssignName(role, x.Dice(6).Add(x.Dice(6)).Add(x.Dice(6)));

                    x.AssignName(soule, x.GetNamed<int>(soule).Substract(x.Combine(x.GetNamed<int>(role), x.GetNamed<int>(value), (r, v) => table[r - 3, v])));


                    return x.GetNamed<int>(soule).GreaterOrEqual(x.Const(50));
                });

                return x.GetNamed<int>(counter);

                //int counter = 0;
                //int soule = 100;
                //while (true)
                //{
                //    counter++;

                //    int coruption = 100 - soule;
                //    var value = 150 + 2 * coruption - soule;


                //    var results = Enumerable.Range(0, 18 - 2).Select(x => (x + 3, this.table[x, value])).ToArray();
                //    var d = (3 * this.D6).Acumulate(results);
                //    int malus = d;
                //    //role -= 3; // 3 is minimum we need to access index 0:
                //    soule -= malus;
                //    //soule -= this.table[role, value];
                //    if (soule < 50)
                //        return counter;


                //}
            });

            var parsedDiceRole = Dice.Parser.SimpleParser.ParseExpression();

            Console.WriteLine($"Configuration took {watch.Elapsed}");

            Console.WriteLine("simple Rolle");
            PrintResults(simpleDiceRole);

            Console.WriteLine("simple if");
            PrintResults(simpleIf);

            Console.WriteLine("double if");
            PrintResults(doubleIf);

            Console.WriteLine("simple while");
            PrintResults(simpleWhile);

            Console.WriteLine("simple while 2");
            PrintResults(simpleWhile2);

            Console.WriteLine("multi Dice roll");
            PrintResults(multiDiceRoll);

            Console.WriteLine("Masive role");
            PrintResults(massiveRole);

            Console.WriteLine("The Dark Eye role");
            PrintResults(theDarkEyeRole);

            Console.WriteLine("Nota Miasma");
            PrintResults(notaMiasma);

            Console.WriteLine("Parsed");
            PrintResults(parsedDiceRole);


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
                //watch.Restart();
            }
            Console.WriteLine($"GetElement took {watch.Elapsed}");
            watch.Restart();
            enumerator.DisposeAsync().AsTask().Wait();
            Console.WriteLine($"Sum {sum}");
            Console.ReadKey(true);
        }

        private static int[,] T()
        {
            const string Path = @"Table.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    // Choose one of either 1 or 2:

                    // 2. Use the AsDataSet extension method
                    using (var result = reader.AsDataSet())
                    {


                        result.Tables.ToString();

                        var table = new int[result.Tables[0].Rows[0].ItemArray.Length - 1, result.Tables[0].Rows.Count - 1];

                        for (int x = 0; x < table.GetLength(0); x++)
                            for (int y = 0; y < table.GetLength(1); y++)
                            {
                                table[x, y] = (int)(double)result.Tables[0].Rows[y + 1].ItemArray[x + 1];
                            }

                        return table;
                    }

                    //Console.WriteLine("Pres ANY key to continue");
                    //Console.ReadKey(true);

                    // The result of each spreadsheet is in result.Tables
                }
            }

        }
    }
}
