using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public abstract class DiceCalculator<T> : BaseDiceCalculator<T>
    {
        internal new WAutomata<T> Automata => base.Automata as WAutomata<T>;

        protected abstract T RoleCalculation();

        public Task<IList<ResultEntry<T>>> DoIt(System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T>>> DoIt(IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            Automata.Configuration = configuration;

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    result = RoleCalculation();
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1> : BaseDiceCalculator<T, P1>
    {
        internal new WAutomata<T, P1> Automata => base.Automata as WAutomata<T, P1>;


        protected abstract T RoleCalculation(P1 p1);


        public Task<IList<ResultEntry<T, P1>>> DoIt(IList<P1> list1, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(list1, null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T, P1>>> DoIt(IList<P1> list1, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            Automata.Configuration = configuration;

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(Automata, list1);


                    result = RoleCalculation(d1);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2> : BaseDiceCalculator<T, P1, P2>
    {
        internal new WAutomata<T, P1, P2> Automata => base.Automata as WAutomata<T, P1, P2>;

        protected abstract T RoleCalculation(P1 p1, P2 p2);


        public Task<IList<ResultEntry<T, P1, P2>>> DoIt(IList<P1> list1, IList<P2> list2, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(list1, list2, null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T, P1, P2>>> DoIt(IList<P1> list1, IList<P2> list2, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {

            Automata.Configuration = configuration;
            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(Automata, list1);
                    P2 d2 = new D<T, P2>(Automata, list2);


                    result = RoleCalculation(d1, d2);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3> : BaseDiceCalculator<T, P1, P2, P3>
    {
        internal new WAutomata<T, P1, P2, P3> Automata => base.Automata as WAutomata<T, P1, P2, P3>;

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3);

        public Task<IList<ResultEntry<T, P1, P2, P3>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(list1, list2, list3, null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {

            Automata.Configuration = configuration;
            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(Automata, list1);
                    P2 d2 = new D<T, P2>(Automata, list2);
                    P3 d3 = new D<T, P3>(Automata, list3);


                    result = RoleCalculation(d1, d2, d3);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3, P4> : BaseDiceCalculator<T, P1, P2, P3, P4>
    {
        internal new WAutomata<T, P1, P2, P3, P4> Automata => base.Automata as WAutomata<T, P1, P2, P3, P4>;

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3, P4 p4);


        public Task<IList<ResultEntry<T, P1, P2, P3, P4>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(list1, list2, list3, list4, null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3, P4>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3, P4>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {

            Automata.Configuration = configuration;
            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(Automata, list1);
                    P2 d2 = new D<T, P2>(Automata, list2);
                    P3 d3 = new D<T, P3>(Automata, list3);
                    P4 d4 = new D<T, P4>(Automata, list4);


                    result = RoleCalculation(d1, d2, d3, d4);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3, P4, P5> : BaseDiceCalculator<T, P1, P2, P3, P4, P5>
    {
        internal new WAutomata<T, P1, P2, P3, P4, P5> Automata => base.Automata as WAutomata<T, P1, P2, P3, P4, P5>;

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);


        public Task<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, IList<P5> list5, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {
            return DoIt(list1, list2, list3, list4, list5, null, default(TimeSpan), null, default(TimeSpan), cancel, configuration);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, IList<P5> list5, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken), DiceCalculatorConfiguration configuration = null)
        {

            Automata.Configuration = configuration;
            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(Automata, list1);
                    P2 d2 = new D<T, P2>(Automata, list2);
                    P3 d3 = new D<T, P3>(Automata, list3);
                    P4 d4 = new D<T, P4>(Automata, list4);
                    P5 d5 = new D<T, P5>(Automata, list5);

                    result = RoleCalculation(d1, d2, d3, d4, d5);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = Automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = Automata.CalculatedPosibilitys;
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (Automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return Automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }

}
