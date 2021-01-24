using Dice;
using ExcelDataReader;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SampleApp
{
    class Program
    {
        static async Task Main(string[] args)
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

            var inputDiceRoll = Calculator<int>.Configure(x =>
            {

                return x.Dice(6).Multiply(x.GetInput());
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
            });

            var parsedDiceRole = Dice.Parser.SimpleParser.ParseExpression<int>("return 3D3");

            Console.WriteLine($"Configuration took {watch.Elapsed}");

            Console.WriteLine("simple Rolle");
            await PrintResults(simpleDiceRole);

            Console.WriteLine("simple if");
            await PrintResults(simpleIf);

            Console.WriteLine("double if");
            await PrintResults(doubleIf);

            Console.WriteLine("simple while");
            await PrintResults(simpleWhile);

            Console.WriteLine("simple while 2");
            await PrintResults(simpleWhile2);

            Console.WriteLine("simple while 3");
            await PrintResults(simpleWhile3);

            Console.WriteLine("input");
            await PrintResults(inputDiceRoll, 1, 2, 3);

            Console.WriteLine("multi Dice roll");
            await PrintResults(multiDiceRoll);

            Console.WriteLine("Masive role");
            await PrintResults(massiveRole);

            Console.WriteLine("The Dark Eye role");
            await PrintResults(theDarkEyeRole);

            Console.WriteLine("Nota Miasma");
            await PrintResults(notaMiasma);

            Console.WriteLine("Parsed");
            await PrintResults(parsedDiceRole);


            //await foreach (var item in asyncEnumerable)
            //{
            //    Console.WriteLine($"{item.Result}:\t{(item.Propability * 100):00.00}%");
            //    sum += item.Propability;
            //}
            Console.WriteLine("FINISHED");
            Console.ReadKey(true);
        }

        private static async Task PrintResults<TResult, TInput>(IExecutor<TResult, TInput> executor, params TInput[] inputs)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var asyncEnumerable = executor.Calculate(inputs);
            double sum = 0;

            //Console.WriteLine($"GetEnumerator Took {watch.Elapsed}");
            watch.Restart();
            Console.WriteLine();
            if (inputs.Length == 0)
            {
                Console.WriteLine($"╔════════╤═════════════╗");
                Console.WriteLine($"║ Result │ Probability ║");
                Console.WriteLine($"╟────────┼─────────────╢");

            }
            else
            {
                Console.WriteLine($"╔════════╤════════╤═════════════╗");
                Console.WriteLine($"║ Intput │ Result │ Probability ║");
                Console.WriteLine($"╟────────┼────────┼─────────────╢");

            }

            await foreach (var item in asyncEnumerable)
            {
                if (inputs.Length == 0)
                    Console.WriteLine($"║ {item.Result,6} │ {item.Propability * 100,10:00.00}% ║");
                else
                    Console.WriteLine($"║ {item.Input,6} │ {item.Result,6} │ {item.Propability * 100,10:00.00}% ║");
                sum += item.Propability;

                //Console.WriteLine($"GetElement took {watch.Elapsed}");
                //watch.Restart();
            }
            if (inputs.Length == 0)
            {
                Console.WriteLine($"╚════════╧═════════════╝");
            }
            else
            {
                Console.WriteLine($"╚════════╧════════╧═════════════╝");
            }


            Console.WriteLine();
            Console.WriteLine($"GetElement took {watch.Elapsed}");
            watch.Restart();
            Console.WriteLine($"Sum {sum}");
            Console.WriteLine();
            Console.Write("Press Any Key to continue...");
            Console.ReadKey(true);
            Console.WriteLine();
            Console.WriteLine();
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
                        if (result is null)
                            throw new IOException($"Could not parse {Path}");

                        result.Tables.ToString();

                        var table = new int[result.Tables[0].Rows[0].ItemArray.Length - 1, result.Tables[0].Rows.Count - 1];

                        for (int x = 0; x < table.GetLength(0); x++)
                            for (int y = 0; y < table.GetLength(1); y++)
                            {
                                var currentValue = result.Tables[0].Rows[y + 1].ItemArray[x + 1];
                                if (currentValue is null)
                                    throw new IOException($"Could not parse {Path} row {y} collumn {x}");

                                table[x, y] = (int)(double)currentValue;
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
