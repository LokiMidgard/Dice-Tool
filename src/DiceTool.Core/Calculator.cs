using System;
using System.Collections.Generic;
using System.Linq;
using Dice.States;

namespace Dice
{
    public class Calculator<TIn>
    {

        public static IExecutor<TResult, TIn> Configure<TResult>(Func<Composer<TIn>, P<TResult>> configurations)
        {
            var composer = new Composer<TIn>();
            var item = configurations(composer);
            return new Executor<TResult, TIn>(item, composer);
        }

    }






}
