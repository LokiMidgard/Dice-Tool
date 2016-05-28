using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dice
{
    public static class ResultEntryExtensions
    {

        #region Variance
        public static double Variance(this IEnumerable<ResultEntry<double>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2 - Math.Pow(average, 2);
        }
        public static double Variance<T>(this IEnumerable<ResultEntry<T>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2 - Math.Pow(average, 2);
        }

        public static IEnumerable<Tuple<double, P1>> Variance<P1>(this IEnumerable<ResultEntry<double, P1>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => x.Item2,
                     x => x.Item2,
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2));
        }
        public static IEnumerable<Tuple<double, P1>> Variance<T, P1>(this IEnumerable<ResultEntry<T, P1>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => x.Item2,
                     x => x.Item2,
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2));
        }

        public static IEnumerable<Tuple<double, P1, P2>> Variance<P1, P2>(this IEnumerable<ResultEntry<double, P1, P2>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item3, x.Item2),
                     x => Tuple.Create(x.Item3, x.Item2),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3));
        }
        public static IEnumerable<Tuple<double, P1, P2>> Variance<T, P1, P2>(this IEnumerable<ResultEntry<T, P1, P2>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item3, x.Item2),
                     x => Tuple.Create(x.Item3, x.Item2),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3>> Variance<P1, P2, P3>(this IEnumerable<ResultEntry<double, P1, P2, P3>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item4, x.Item2, x.Item3),
                     x => Tuple.Create(x.Item4, x.Item2, x.Item3),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4));
        }
        public static IEnumerable<Tuple<double, P1, P2, P3>> Variance<T, P1, P2, P3>(this IEnumerable<ResultEntry<T, P1, P2, P3>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item4, x.Item2, x.Item3),
                     x => Tuple.Create(x.Item4, x.Item2, x.Item3),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3, P4>> Variance<P1, P2, P3, P4>(this IEnumerable<ResultEntry<double, P1, P2, P3, P4>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item5, x.Item2, x.Item3, x.Item4),
                     x => Tuple.Create(x.Item5, x.Item2, x.Item3, x.Item4),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4, x1.Item5));
        }
        public static IEnumerable<Tuple<double, P1, P2, P3, P4>> Variance<T, P1, P2, P3, P4>(this IEnumerable<ResultEntry<T, P1, P2, P3, P4>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item5, x.Item2, x.Item3, x.Item4),
                     x => Tuple.Create(x.Item5, x.Item2, x.Item3, x.Item4),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4, x1.Item5));
        }
        public static IEnumerable<Tuple<double, P1, P2, P3, P4, P5>> Variance<P1, P2, P3, P4, P5>(this IEnumerable<ResultEntry<double, P1, P2, P3, P4, P5>> results)
        {
            var average = results.Average();
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(x.Result, 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item6, x.Item2, x.Item3, x.Item4, x.Item5),
                     x => Tuple.Create(x.Item6, x.Item2, x.Item3, x.Item4, x.Item5),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4, x1.Item5, x1.Item6));
        }
        public static IEnumerable<Tuple<double, P1, P2, P3, P4, P5>> Variance<T, P1, P2, P3, P4, P5>(this IEnumerable<ResultEntry<T, P1, P2, P3, P4, P5>> results, Func<T, double> numericTranslation)
        {
            var average = results.Average(numericTranslation);
            var average2 = results
                .Select(x => x.WithResult(Math.Pow(numericTranslation(x.Result), 2)))
                .Average();
            return average2
                 .Join(
                     average,
                     x => Tuple.Create(x.Item6, x.Item2, x.Item3, x.Item4, x.Item5),
                     x => Tuple.Create(x.Item6, x.Item2, x.Item3, x.Item4, x.Item5),
                     (x1, x2) =>
                         Tuple.Create(x1.Item1 - Math.Pow(x2.Item1, 2), x1.Item2, x1.Item3, x1.Item4, x1.Item5, x1.Item6));
        }

        #endregion


        #region Average
        public static double Average(this IEnumerable<ResultEntry<double>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.Sum(x => x.Propability / propabilitySum * x.Result);
        }
        public static double Average<T>(this IEnumerable<ResultEntry<T>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result));
        }


        public static IEnumerable<Tuple<double, P1>> Average<P1>(this IEnumerable<ResultEntry<double, P1>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => x.Item1).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * x.Result), y.Key));
        }

        public static IEnumerable<Tuple<double, P1>> Average<T, P1>(this IEnumerable<ResultEntry<T, P1>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => x.Item1).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result)), y.Key));
        }

        public static IEnumerable<Tuple<double, P1, P2>> Average<P1, P2>(this IEnumerable<ResultEntry<double, P1, P2>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * x.Result), y.Key.Item1, y.Key.Item2));
        }

        public static IEnumerable<Tuple<double, P1, P2>> Average<T, P1, P2>(this IEnumerable<ResultEntry<T, P1, P2>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result)), y.Key.Item1, y.Key.Item2));
        }


        public static IEnumerable<Tuple<double, P1, P2, P3>> Average<P1, P2, P3>(this IEnumerable<ResultEntry<double, P1, P2, P3>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * x.Result), y.Key.Item1, y.Key.Item2, y.Key.Item3));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3>> Average<T, P1, P2, P3>(this IEnumerable<ResultEntry<T, P1, P2, P3>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result)), y.Key.Item1, y.Key.Item2, y.Key.Item3));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3, P4>> Average<P1, P2, P3, P4>(this IEnumerable<ResultEntry<double, P1, P2, P3, P4>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * x.Result), y.Key.Item1, y.Key.Item2, y.Key.Item3, y.Key.Item4));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3, P4>> Average<T, P1, P2, P3, P4>(this IEnumerable<ResultEntry<T, P1, P2, P3, P4>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result)), y.Key.Item1, y.Key.Item2, y.Key.Item3, y.Key.Item4));
        }
        public static IEnumerable<Tuple<double, P1, P2, P3, P4, P5>> Average<P1, P2, P3, P4, P5>(this IEnumerable<ResultEntry<double, P1, P2, P3, P4, P5>> results)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * x.Result), y.Key.Item1, y.Key.Item2, y.Key.Item3, y.Key.Item4, y.Key.Item5));
        }

        public static IEnumerable<Tuple<double, P1, P2, P3, P4, P5>> Average<T, P1, P2, P3, P4, P5>(this IEnumerable<ResultEntry<T, P1, P2, P3, P4, P5>> results, Func<T, double> numericTranslation)
        {
            var propabilitySum = results.Sum(x => x.Propability);
            return results.GroupBy(x => Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, x.Item5)).Select(y => Tuple.Create(y.Sum(x => x.Propability / propabilitySum * numericTranslation(x.Result)), y.Key.Item1, y.Key.Item2, y.Key.Item3, y.Key.Item4, y.Key.Item5));
        }


        #endregion
    }
}
