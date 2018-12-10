using System;


namespace Dice
{
    public class ResultEntry<TResult>
    {
        private readonly TResult result;
        private readonly Exception? exception;

        public TResult Result
        {
            get => this.State != ResultState.Completed
                ? throw new InvalidOperationException()
                : result!;
        }

        public Exception Exception
        {
            get => this.State != ResultState.Faulted
                ? throw new InvalidOperationException()
                : exception!;
        }

        public ResultState State { get; }

        public double Propability { get; }


        internal ResultEntry(TResult result, double propability)
        {
            this.State = ResultState.Completed;
            this.result = result;
            this.Propability = propability;
        }
        internal ResultEntry(Exception result, double propability)
        {
            this.State = ResultState.Faulted;
            this.exception = result;
            this.Propability = propability;
            this.result = default!; // if State is non Completed, we never access result
        }

        internal ResultEntry(double propability) // only for cancled.
        {
            this.Propability = propability;
            this.State = ResultState.Canceld;
            this.result = default!; // if State is non Completed, we never access result
        }



    }
}
