using System;


namespace Dice
{
    public class ResultEntry<TResult>
    {
        private readonly TResult result;
        private readonly double propability;
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

        public double Propability => this.propability;

        private readonly Exception? exception;

        internal ResultEntry(TResult result, double propability)
        {
            this.State = ResultState.Completed;
            this.result = result;
            this.propability = propability;
        }
        internal ResultEntry(Exception result, double propability)
        {
            this.State = ResultState.Faulted;
            this.exception = result;
            this.propability = propability;
            this.result = default!; // if State is non Completed, we never access result
        }

        internal ResultEntry(double propability) // only for cancled.
        {
            this.propability = propability;
            this.State = ResultState.Canceld;
            this.result = default!; // if State is non Completed, we never access result
        }



    }
}
