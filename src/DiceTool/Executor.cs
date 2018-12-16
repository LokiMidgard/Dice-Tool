using Dice.States;
using Dice.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dice
{


    public interface IExecutor<TResult, Tin>
    {

        IAsyncEnumerable<ResultEntry<TResult>> Calculate(Tin input);

    }

    internal class Executor<TResult, TIn> : IExecutor<TResult, TIn>
    {
        private readonly IEnumerable<(P<TResult> Variable, State lastState)> results;

        public Executor(IEnumerable<(P<TResult> Variable, State lastState)> result)
        {
            System.Diagnostics.Debug.Assert(result.Count() == 1);
            this.results = result;
        }

        public IAsyncEnumerable<ResultEntry<TResult>> Calculate(TIn input)
        {
            //throw new System.Exception();

            return new Wraper(this.CalculateInternal(input));

        }


        public double Epsylon { get; set; } = 0.000000001;


        private IEnumerable<ResultEntry<TResult>> CalculateInternal(TIn input)
        {

            var sum = new Dictionary<TResult, double>();

            // prepeare Optimization
            foreach (var (Variable, lastState) in this.results)
                lastState.PrepareOptimize(Enumerable.Repeat(Variable as IP, 1));

            foreach (var (variable, state) in this.results)
            {
                var choiseManager = new ChoiseManager();
                var whileManager = new WhileManager(choiseManager);


                //int counter = 0;

                while (!choiseManager.IsCompleted && Math.Abs(choiseManager.SolvedPropability - 1) > this.Epsylon)
                {
                    using (choiseManager.EnableMutation())
                        state.PreCalculatePath(whileManager);

                    state.Optimize(whileManager);


                    var statePropability = state.GetStatePropability(whileManager);
                    var table = state.GetTable(variable, whileManager);
                    var currentSum = 0.0;
                    var tableCount = table.GetCount();
                    for (int i = 0; i < tableCount; i++)
                    {
                        var value = table.GetValue(variable, i);
                        var p = table.GetValue(Table.PropabilityKey, i);
                        if (!sum.ContainsKey(value))
                            sum.Add(value, 0.0);
                        var propability = p * statePropability;
                        currentSum += propability;
                        sum[value] += propability;
                    }
                    foreach (var item in sum.OrderBy(x => x.Value))
                    {
                        yield return new ResultEntry<TResult>(item.Key, item.Value, currentSum);
                    }
                    sum.Clear();
                    choiseManager.Terminate(currentSum);
                    //System.Console.WriteLine($"Terminated {++counter} run. Searched {choiseManager.SolvedPropability}");
                    //if (counter++ > 3)
                    //    break;
                }


            }
        }

        private class Wraper : IAsyncEnumerable<ResultEntry<TResult>>
        {
            private readonly IEnumerable<ResultEntry<TResult>> enumerable;

            public Wraper(IEnumerable<ResultEntry<TResult>> enumerable)
            {
                this.enumerable = enumerable;
            }

            public IAsyncEnumerator<ResultEntry<TResult>> GetAsyncEnumerator()
            {
                return new Enumerator(this);
            }

            private class Enumerator : IAsyncEnumerator<ResultEntry<TResult>>
            {
                private readonly IEnumerator<ResultEntry<TResult>> original;

                public Enumerator(Wraper wraper)
                {
                    this.original = wraper.enumerable.GetEnumerator();
                }

                public ResultEntry<TResult> Current => this.original.Current;

                public ValueTask DisposeAsync()
                {
                    this.original.Dispose();
                    return new ValueTask();
                }

                public ValueTask<bool> MoveNextAsync()
                {
                    var result = this.original.MoveNext();
                    return new ValueTask<bool>(result);
                }
            }
        }
    }
}