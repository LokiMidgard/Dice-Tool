using Dice.States;
using Dice.Tables;
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
            this.results = result;
        }

        public IAsyncEnumerable<ResultEntry<TResult>> Calculate(TIn input)
        {
            //throw new System.Exception();

            return new Wraper(this.CalculateInternal(input));

        }

        private void Optimize<TResult>(IEnumerable<(P<TResult> result, State state, WhileManager manager)> resultData)
        {
            foreach (var (p, state, manager) in resultData)
                state.PrepareOptimize(Enumerable.Repeat(p, 1).Cast<IP>(), manager);

            foreach (var (_, state, manager) in resultData)
                state.Optimize(manager);


        }


        private IEnumerable<ResultEntry<TResult>> CalculateInternal(TIn input)
        {
            var sum = new Dictionary<TResult, double>();

            int whileIndex = 1;
            //            Optimize(results);

            foreach (var (variable, state) in this.results)
            {
                IWhileManager initialState = new InitialWhileManager(whileIndex, state.WhileCount);
                var whileManager = new WhileManager(initialState, -1, null!, 0);
                var table = state.GetTable(variable, whileManager);

                for (int i = 0; i < table.GetCount(); i++)
                {
                    var value = table.GetValue(variable, i);
                    var p = table.GetValue(Table.PropabilityKey, i);
                    if (!sum.ContainsKey(value))
                        sum.Add(value, 0.0);
                    sum[value] += p * state.GetStatePropability(whileManager);
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