using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public class ResultEntry<T, TParam1, TParam2, TParam3, TParam4, TParam5> : Tuple<TParam1, TParam2, TParam3, TParam4, TParam5>, IResultEntry<T>
    {
        public ResultEntry(double factor, T result, TParam1 item1, TParam2 item2, TParam3 item3, TParam4 item4, TParam5 item5) : base(item1, item2, item3, item4, item5)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T, TParam1, TParam2, TParam3, TParam4, TParam5> WithResult<T>(T v) => new ResultEntry<T, TParam1, TParam2, TParam3, TParam4, TParam5>(Propability, v, Item1, Item2, Item3, Item4, Item5);
    }
    public class ResultEntry<T, TParam1, TParam2, TParam3, TParam4> : Tuple<TParam1, TParam2, TParam3, TParam4>, IResultEntry<T>
    {
        public ResultEntry(double factor, T result, TParam1 item1, TParam2 item2, TParam3 item3, TParam4 item4) : base(item1, item2, item3, item4)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T, TParam1, TParam2, TParam3, TParam4> WithResult<T>(T v) => new ResultEntry<T, TParam1, TParam2, TParam3, TParam4>(Propability, v, Item1, Item2, Item3, Item4);
    }
    public class ResultEntry<T, TParam1, TParam2, TParam3> : Tuple<TParam1, TParam2, TParam3>, IResultEntry<T>
    {
        public ResultEntry(double factor, T result, TParam1 item1, TParam2 item2, TParam3 item3) : base(item1, item2, item3)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T, TParam1, TParam2, TParam3> WithResult<T>(T v) => new ResultEntry<T, TParam1, TParam2, TParam3>(Propability, v, Item1, Item2, Item3);
    }
    public class ResultEntry<T, TParam1, TParam2> : Tuple<TParam1, TParam2>, IResultEntry<T>
    {
        public ResultEntry(double factor, T result, TParam1 item1, TParam2 item2) : base(item1, item2)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T, TParam1, TParam2> WithResult<T>(T v) => new ResultEntry<T, TParam1, TParam2>(Propability, v, Item1, Item2);
    }
    public class ResultEntry<T, TParam1> : Tuple<TParam1>, IResultEntry<T>
    {
        public ResultEntry(double factor, T result, TParam1 item1) : base(item1)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T, TParam1> WithResult<T>(T v) => new ResultEntry<T, TParam1>(Propability, v, Item1);
    }
    public class ResultEntry<T> : IResultEntry<T>
    {
        public ResultEntry(double factor, T result)
        {
            Propability = factor;
            Result = result;
        }
        public double Propability { get; }
        public T Result { get; }

        internal ResultEntry<T> WithResult<T>(T v) => new ResultEntry<T>(Propability, v);

    }
}
