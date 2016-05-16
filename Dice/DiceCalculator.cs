using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public abstract class DiceCalculator<T> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation();

        public Task<IList<ResultEntry<T>>> DoIt( System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt( null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T>>> DoIt(IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

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
                        var data = automata.GetDistribution();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation(P1 p1);


        public Task<IList<ResultEntry<T, P1>>> DoIt(IList<P1> list1, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt(list1,null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T, P1>>> DoIt(IList<P1> list1,IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(automata, list1);
        

                    result = RoleCalculation(d1);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = automata.GetDistribution<P1>();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution<P1>();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation(P1 p1, P2 p2);


        public Task<IList<ResultEntry<T, P1, P2>>> DoIt(IList<P1> list1, IList<P2> list2, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt(list1, list2, null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T, P1, P2>>> DoIt(IList<P1> list1, IList<P2> list2, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(automata, list1);
                    P2 d2 = new D<T, P2>(automata, list2);


                    result = RoleCalculation(d1, d2);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = automata.GetDistribution<P1, P2>();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution<P1, P2>();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3);

        public Task<IList<ResultEntry<T, P1, P2, P3>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3,  System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt(list1, list2, list3, null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(automata, list1);
                    P2 d2 = new D<T, P2>(automata, list2);
                    P3 d3 = new D<T, P3>(automata, list3);
    

                    result = RoleCalculation(d1, d2, d3);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = automata.GetDistribution<P1, P2, P3>();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution<P1, P2, P3>();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3, P4> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3, P4 p4);


        public Task<IList<ResultEntry<T, P1, P2, P3, P4>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4,  System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt(list1, list2, list3, list4,  null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3, P4>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4,  IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3, P4>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(automata, list1);
                    P2 d2 = new D<T, P2>(automata, list2);
                    P3 d3 = new D<T, P3>(automata, list3);
                    P4 d4 = new D<T, P4>(automata, list4);
            

                    result = RoleCalculation(d1, d2, d3, d4);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = automata.GetDistribution<P1, P2, P3, P4>();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution<P1, P2, P3, P4>();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }
    public abstract class DiceCalculator<T, P1, P2, P3, P4, P5> : BaseDiceCalculato<T>
    {

        protected abstract T RoleCalculation(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);


        public Task<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, IList<P5> list5, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {
            return DoIt(list1, list2, list3, list4, list5, null, default(TimeSpan), null, default(TimeSpan), cancel);
        }

        public Task<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> DoIt(IList<P1> list1, IList<P2> list2, IList<P3> list3, IList<P4> list4, IList<P5> list5, IProgress<int> progressSimple, TimeSpan reportIntervallSimple, IProgress<IList<ResultEntry<T, P1, P2, P3, P4, P5>>> progress, TimeSpan reportIntervall, System.Threading.CancellationToken cancel = default(System.Threading.CancellationToken))
        {

            var erg = Task.Factory.StartNew(() =>
            {
                DateTime start = DateTime.Now;
                DateTime startSimple = DateTime.Now;
                T result;
                do
                {
                    P1 d1 = new D<T, P1>(automata, list1);
                    P2 d2 = new D<T, P2>(automata, list2);
                    P3 d3 = new D<T, P3>(automata, list3);
                    P4 d4 = new D<T, P4>(automata, list4);
                    P5 d5 = new D<T, P5>(automata, list5);

                    result = RoleCalculation(d1, d2, d3, d4, d5);
                    if (progress != null && DateTime.Now - start > reportIntervall)
                    {
                        var data = automata.GetDistribution<P1, P2, P3, P4, P5>();
                        progress.Report(data);
                        start = DateTime.Now;
                    }
                    if (progressSimple != null && DateTime.Now - startSimple > reportIntervallSimple)
                    {
                        var data = automata.GetCalculatedPosibilitys();
                        progressSimple.Report(data);
                        startSimple = DateTime.Now;
                    }
                } while (automata.NextRoll(result) && !cancel.IsCancellationRequested);

                return automata.GetDistribution<P1, P2, P3, P4, P5>();
            }, TaskCreationOptions.LongRunning);
            return erg;
        }
    }

}
