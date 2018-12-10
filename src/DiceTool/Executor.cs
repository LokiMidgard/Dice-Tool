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

        private IEnumerable<ResultEntry<TResult>> CalculateInternal(TIn input)
        {
            var sum = new Dictionary<TResult, double>();

            foreach (var (variable, state) in this.results)
            {
                var table = state.GetTable(variable);

                for (int i = 0; i < table.Count; i++)
                {
                    var value = table.GetValue(variable, i);
                    var p = table.GetValue(Table.PropabilityKey, i);
                    if (!sum.ContainsKey(value))
                        sum.Add(value, 0.0);
                    sum[value] += p * state.StatePropability;
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