using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp;
using Dice;

namespace DiceConsoleTest
{
    class Program
    {
        private static readonly Random r = new Random();

        static void Main(string[] args)
        {
            // var samples = Generate().ToArray();

            //Compilation test = CreateTestCompilation();

            //foreach (SyntaxTree sourceTree in test.SyntaxTrees)
            //{
            //    SemanticModel model = test.GetSemanticModel(sourceTree);

            //    var te = WRewriter.Rewrite(model, sourceTree);

            //    var rewriter = new YieldRewriter(model);

            //    SyntaxNode newSource = rewriter.Visit(te);



            //    if (newSource != sourceTree.GetRoot())
            //    {
            //        var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
            //        Console.WriteLine(Formatter.Format(newSource, workspace).ToFullString());
            //    }
            //    else
            //    {
            //        Console.WriteLine("No Changes");
            //    }
            //}
            int counter = 0;
            var busyIndicator = new char[]{'-','\\','|','/' };
            var t = new TestImpl();
            var task = t.DoIt(Enumerable.Range(8, 1).ToArray(), Enumerable.Range(8, 1).ToArray(), Enumerable.Range(8, 1).ToArray(), Enumerable.Range(0, 21).ToArray(), Enumerable.Range(0, 1).ToArray(),new Progress<int>(progres=>
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.WriteLine($"{busyIndicator[unchecked(counter++)%busyIndicator.Length]}  Bisher {progres,0:n} berechnet.            ");
                //Console.WriteLine($"Geschätzt \n{progres.Max(x=>x.Factor),0:n}                    ");
            }),TimeSpan.FromSeconds(2),null,default(TimeSpan));

            

            var erg = task.Result;

            foreach (var item in erg.Where(x => x.Result).Where(x => x.Item1 == 8 && x.Item2 == 8 && x.Item3 == 8).Where(x => x.Item5 == 0).GroupBy(x => x.Item4).OrderBy(x => x.Key).Select(x=> new { Percentage = x.Sum(y => y.Factor), Value=x.Key}))
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

                int i =  2 * W6;
                return i == taw;

                //int d1 = W20;
                //int d2 = W20;
                //int d3 = W20;

                //taw -= dificulty;

                //if (taw < 0)
                //{
                //    e1 += taw;
                //    e2 += taw;
                //    e3 += taw;
                //    taw = 0;
                //}

                //taw -= (Math.Max(0, d1 - e1) + Math.Max(0, d2 - e2) + Math.Max(0, d3 - e3));
                //return taw >= 0;
            }


        }




        private static Compilation CreateTestCompilation()
        {
            //String programPath = @"..\..\Program.cs";
            //String programText = File.ReadAllText(programPath);
            //SyntaxTree programTree =
            //               CSharpSyntaxTree.ParseText(programText)
            //                               .WithFilePath(programPath);

            //String rewriterPath = @"..\..\TypeInferenceRewriter.cs";
            //String rewriterText = File.ReadAllText(rewriterText);
            //SyntaxTree rewriterTree =
            //               CSharpSyntaxTree.ParseText(rewriterText)
            //                               .WithFilePath(rewriterPath);

            var programTree = CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));

            SyntaxTree[] sourceTrees = { programTree };

            MetadataReference mscorlib =
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            MetadataReference wLib =
                    MetadataReference.CreateFromFile(typeof(D<object>).Assembly.Location);
            MetadataReference csharpCodeAnalysis =
                    MetadataReference.CreateFromFile(typeof(CSharpSyntaxTree).Assembly.Location);

            MetadataReference[] references = { mscorlib, wLib/*, codeAnalysis, csharpCodeAnalysis*/ };

            return CSharpCompilation.Create("TransformationCS",
                                            sourceTrees,
                                            references,
                                            new CSharpCompilationOptions(
                                                    OutputKind.ConsoleApplication));

        }

        const string code = @"
      public static readonly W W20 = new W();



        public static int Generate()
        {
            const int y1 = 14;
            const int y2 = 14;
            const int y3 = 14;

            const int taw = 13;
            const int dif = 7;


            var x1 = W20;
            W x2 = W20;
            var x3 = W20;
            

                var z1 = y1;
                var z2 = y2;
                var z3 = y3;

                var effectivTaw = taw - dif;
                if (effectivTaw < 0)
                {
                    z1 += effectivTaw;
                    z2 += effectivTaw;
                    z3 += effectivTaw;

                    effectivTaw = 0;
                }

                if (x1 > z1)
                    effectivTaw -= x1 - z1;
                if (x2 > z2)
                    effectivTaw -= x2 - z2;
                if (x3 > z3)
                    effectivTaw -= x3 - z3;

                if (effectivTaw < 0)
                    return 0;
                else
                    return Math.Max(1, effectivTaw);

            

        }";

        public struct Datatype2D
        {

            public Datatype2D(int x) : this()
            {
                this.X = x;
            }

            public int X { get; }
        }


        private static IEnumerable<int> D(int count, int size)
        {
            int[] erg = { 0 };
            for (int i = 0; i < count; i++)
            {
                var tmp = new int[erg.Length * size];
                var d = D(size);
                for (int j = 0; j < erg.Length; j++)
                    for (int k = 0; k < d.Length; k++)
                    {
                        tmp[j * d.Length + k] = erg[j] + d[k];
                    }
                erg = tmp;
            }
            return erg;

        }


        public static string Translate(string s)
        {
            if (s.Contains('_'))
                throw new ArgumentException("May not contains '_'. This is a reserverd character");



            return s;

        }


        private static int[] D(int size)
        {
            var a = new int[size];

            for (int i = 0; i < size; i++)
            {
                a[i] = i + 1;
            }

            for (int i = 0; i < size; i++)
            {
                var j = r.Next(size - i) + i;
                var tmp = a[j];
                a[j] = a[i];
                a[i] = tmp;
            }

            return a;
        }

    }
}
