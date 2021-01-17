using System;


namespace Dice
{
    public class ResultEntry<TResult, TIn>
    {
        private readonly TResult result;
        private readonly Exception? exception;
        private readonly TIn input;

        public TResult Result
        {
            get => this.State != ResultState.Completed
                ? throw new InvalidOperationException()
                : this.result!;
        }

        public Exception Exception
        {
            get => this.State != ResultState.Faulted
                ? throw new InvalidOperationException()
                : this.exception!;
        }

        public ResultState State { get; }

        public TIn Input
        {
            get => this.State == ResultState.Canceld
                     ? throw new InvalidOperationException()
                     : this.input!;
        }

        public double Propability { get; }

        public double CompletePercentage { get; }

        internal ResultEntry(TResult result, TIn input, double propability, double completePercentage)
        {
            this.State = ResultState.Completed;
            this.result = result;
            this.Propability = propability;
            this.CompletePercentage = completePercentage;
            this.input = input;
        }
        internal ResultEntry(Exception result, TIn input, double propability, double completePercentage)
        {
            this.State = ResultState.Faulted;
            this.exception = result;
            this.Propability = propability;
            this.result = default!; // if State is non Completed, we never access result
            this.CompletePercentage = completePercentage;
            this.input = input;
        }

        internal ResultEntry(double propability) // only for cancled.
        {
            this.Propability = propability;
            this.State = ResultState.Canceld;
            this.result = default!; // if State is non Completed, we never access result
            this.input = default!;
        }



    }
}
