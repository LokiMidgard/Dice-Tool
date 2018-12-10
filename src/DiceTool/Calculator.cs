using System;
using System.Collections.Generic;
using Dice.States;

namespace Dice
{
    public class Calculator<TIn>
    {

        public static IExecutor<TResult, TIn> Configure<TResult>(Func<Composer<TIn>, P<TResult>> configurations)
        {
            var composer = new Composer<TIn>();
            var results = new List<(P<TResult> Variable, State lastState)>();
            do
            {
                var item = configurations(composer);
                results.Add((item, composer.CurrentState));
            } while (composer.Reset());

            composer.Optimize(results);

            return new Executor<TResult, TIn>(results);
        }

    }





    
}
