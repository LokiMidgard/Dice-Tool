using Dice.States;
using Dice.Tables;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dice
{


    public interface IExecutor<TResult, Tin>
    {

        IAsyncEnumerable<ResultEntry<TResult, Tin>> Calculate(IEnumerable<Tin> input);
        IAsyncEnumerable<ResultEntry<TResult, Tin>> Calculate(params Tin[] input);
        double Epsylon { get; set; }

    }

    internal class Executor<TResult, TIn> : IExecutor<TResult, TIn>
    {
        private readonly P<TResult> resultVariable;
        private readonly P<TIn> inputVariable;
        private readonly State lastState;
        private readonly Composer<TIn> composer;

        public Executor(P<TResult> variable, Composer<TIn> composer)
        {
            this.composer = composer;
            this.resultVariable = variable;
            this.inputVariable = composer.GetInput();
            this.lastState = new States.CombinationState<TResult, TIn, int>(composer.State.Current, new P<int>(composer, ""), variable, this.inputVariable, (x1, x2) => 0);


            //this.lastState = composer.State.Current;
        }

        public IAsyncEnumerable<ResultEntry<TResult, TIn>> Calculate(IEnumerable<TIn> input) => new Wraper(this.CalculateInternal(input));
        public IAsyncEnumerable<ResultEntry<TResult, TIn>> Calculate(params TIn[] input) => this.Calculate(input as IEnumerable<TIn>);


        public double Epsylon { get; set; } = 0.000000001;



        private IEnumerable<ResultEntry<TResult, TIn>> CalculateInternal(IEnumerable<TIn> input)
        {

            //var sum = new Dictionary<TResult, double>();
            if (!input.Any())
                input = new TIn[1];
            this.composer.Setinput(input);

            // prepeare Optimization
            this.lastState.PrepareOptimize(new IP[] { this.resultVariable, this.inputVariable });


            var choiseManager = new ChoiseManager();
            var whileManager = new WhileManager(choiseManager);


            //int counter = 0;
            double completePropability = 0;
            var inputCount = input.Count();


#if DEBUG
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var outputWatch = System.Diagnostics.Stopwatch.StartNew();
            var statistics = new RunningStatistics();
            uint count=0;
#endif

            while (!choiseManager.IsCompleted && Math.Abs(choiseManager.SolvedPropability / inputCount - 1) > this.Epsylon)
            {
#if DEBUG
                watch.Restart();
                count++;
#endif

                using (choiseManager.EnableMutation())
                    this.lastState.PreCalculatePath(whileManager);

                this.lastState.Optimize(whileManager);


                var currentSum = 0.0;
                var statePropability = this.lastState.GetStatePropability(whileManager);
                var table = this.lastState.GetTable(this.resultVariable, whileManager);
                var tableCount = table.GetCount();
                for (int i = 0; i < tableCount; i++)
                {
                    var value = table.GetValue(this.resultVariable, i);
                    var @in = table.GetValue(this.inputVariable, i);
                    var p = table.GetValue(Table.PropabilityKey, i);
                    //if (!sum.ContainsKey(value))
                    //sum.Add(value, 0.0);
                    var propability = p * statePropability;
                    currentSum += propability;
                    completePropability += propability / inputCount;
                    yield return new ResultEntry<TResult, TIn>(value, @in, propability, completePropability);
                    //sum[value] += propability;
                }
                //foreach (var item in sum.OrderBy(x => x.Value))
                //{
                //    yield return new ResultEntry<TResult, TIn>(item.Key, item.Value, completePropability);
                //}
                //sum.Clear();
                choiseManager.Terminate(currentSum);
#if DEBUG
                statistics.Push(watch.Elapsed.TotalSeconds);
                if (outputWatch.Elapsed > TimeSpan.FromSeconds(10))
                {
                    outputWatch.Restart();
                    System.Diagnostics.Debug.WriteLine($"AVG Took {TimeSpan.FromSeconds(statistics.Mean)} Max {TimeSpan.FromSeconds(statistics.Maximum)} Dev {statistics.StandardDeviation} Count {count}");
                }
#endif
            }
        }

        private class Wraper : IAsyncEnumerable<ResultEntry<TResult, TIn>>
        {
            private readonly IEnumerable<ResultEntry<TResult, TIn>> enumerable;

            public Wraper(IEnumerable<ResultEntry<TResult, TIn>> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IAsyncEnumerator<ResultEntry<TResult, TIn>> GetAsyncEnumerator()
            {
                return new Enumerator(this);
            }

            private class Enumerator : IAsyncEnumerator<ResultEntry<TResult, TIn>>
            {
                private readonly IEnumerator<ResultEntry<TResult, TIn>> original;

                public Enumerator(Wraper wraper)
                {
                    this.original = wraper.enumerable.GetEnumerator();
                }

                public ResultEntry<TResult, TIn> Current => this.original.Current;

                public ValueTask DisposeAsync()
                {
                    this.original.Dispose();
                    return new ValueTask();
                }

                public ValueTask<bool> MoveNextAsync()
                {
                    return new ValueTask<bool>(Task.Run(() => this.original.MoveNext()));
                    //return new ValueTask<bool>(this.original.MoveNext());
                }
            }
        }
    }
}