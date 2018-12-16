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

        private void PrepareOptimize<TResult>(IEnumerable<(P<TResult> result, State state)> resultData)
        {
            // For perpartion we obly need to perform two loops. 
            // This steps only recordes which variables will be used from further down states. So running more then twice will not change the used variables.
            //foreach (var (p, state) in resultData)
            //{
            //    IWhileManager initialState = new InitialWhileManager(1, state.WhileCount);
            //    var manager = new WhileManager(initialState, -1, null!, 0);
            //    state.PrepareOptimize(Enumerable.Repeat(p, 1).Cast<IP>(), manager);
            //}


        }

        public double Epsylon { get; set; } = 0.000000001;


        private IEnumerable<ResultEntry<TResult>> CalculateInternal(TIn input)
        {

            var sum = new Dictionary<TResult, double>();
            //            Optimize(results);
            //this.PrepareOptimize(results);

            // prepeare Optimization
            foreach (var item in results)
                item.lastState.PrepareOptimize(Enumerable.Repeat(item.Variable as IP, 1));

            foreach (var (variable, state) in this.results)
            {
                var choiseManager = new ChoiseManager();
                var whileManager = new WhileManager(choiseManager);


                int counter = 0;

                while (!choiseManager.IsCompleted && Math.Abs(choiseManager.SolvedPropability - 1) > this.Epsylon)
                {
                    using (choiseManager.EnableMutation())
                        state.PreCalculatePath(whileManager);

                    state.Optimize(whileManager);


                    var statePropability = state.GetStatePropability(whileManager);
                    var table = state.GetTable(variable, whileManager);
                    var currentSum = 0.0;
                    for (int i = 0; i < table.GetCount(); i++)
                    {
                        var value = table.GetValue(variable, i);
                        var p = table.GetValue(Table.PropabilityKey, i);
                        if (!sum.ContainsKey(value))
                            sum.Add(value, 0.0);
                        var propability = p * statePropability;
                        currentSum += propability;
                        sum[value] += propability;
                    }

                    choiseManager.Terminate(currentSum);
                    System.Console.WriteLine($"Terminated {++counter} run. Searched {choiseManager.SolvedPropability}");
                    //if (counter++ > 3)
                    //    break;
                }


            }
            foreach (var item in sum.OrderBy(x => x.Value))
            {
                yield return new ResultEntry<TResult>(item.Key, item.Value);
            }
        }

        private class Wraper : IAsyncEnumerable<ResultEntry<TResult>>
        {
            private IEnumerable<ResultEntry<TResult>> enumerable;

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