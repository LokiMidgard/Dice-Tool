using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public class D<T, TParam> : D<T>
    {
        internal D(WAutomataBase<T> automata, IList<TParam> p1, IList<double> propabilityDistribution = null) : base(p1.Count, automata)
        {
            this.Parameters = p1;
            if (propabilityDistribution != null)
            {
                if (p1.Count != propabilityDistribution.Count)
                    throw new ArgumentException("Number of Parameters and Propabilitys must have the same size.");
                this.Propabilitys = propabilityDistribution;
            }
        }

        public IList<TParam> Parameters { get; }

        public static implicit operator TParam(D<T, TParam> w)
        {
            w.automata.Init(w);
            var posibleRoles = w.automata.PosibleRoles().ToArray();
            int role = SelectRole(w);
            w.automata.Role(role);
            return w.Parameters[role - 1];
        }

        public D<T, TOut> Acumulate<TOut>(IEnumerable<(TParam value, TOut result)> results)
        {
            if (!(results is IList<(TParam value, TOut result)> list))
                list = results.ToArray();

            var lookup = results.Select(x => x.value).ToLookup(x => x);
            foreach (var item in this.Parameters)
            {
                if (!lookup.Contains(item))
                    throw new ArgumentException();
                if (lookup[item].Count() != 1)
                    throw new ArgumentException();
            }

            var group = results.GroupBy(x => x.result);

            var r = group.Select(x =>
            {
                var prop = x.Select(y => this.Propabilitys[this.Parameters.IndexOf(y.value)]).Sum();
                return (Propapbility: prop, Result: x.Key);
            });

            return new D<T, TOut>(this.automata, r.Select(x => x.Result).ToList(), r.Select(x => x.Propapbility).ToList());
        }


    }



    public class D<T>
    {
        internal readonly WAutomataBase<T> automata;

        public IList<double> Propabilitys { get; internal set; }



        public int Size { get; }

        internal D(int size, WAutomataBase<T> automata)
        {
            if (automata == null)
                throw new ArgumentNullException(nameof(automata));
            this.Size = size;
            this.automata = automata;
            this.Propabilitys = Enumerable.Repeat(1.0 / size, size).ToArray();
        }


        //public static int operator *(int count, D<T> w)
        //{
        //    int erg = 0;
        //    for (int i = 0; i < count; i++)
        //        erg += w; // implicit cast to int, so we role count dice.
        //    return erg;
        //}


        public static D<T, int> operator *(int count, D<T> w)
        {
            var numbers = Enumerable.Range(1, w.Size);
            var tempNumbers = Enumerable.Repeat(0, 1);
            for (int i = 0; i < count; i++)
                tempNumbers = Cross(tempNumbers, numbers, (x1, x2) => x1 + x2);

            var allVariants = Math.Pow(w.Size, count);

            var group = tempNumbers.GroupBy(x => x).Select(x => new { Number = x.Key, Propability = (x.Count() / allVariants) }).OrderBy(x => x.Number).ToArray();

            var distribution = group.Select(x => x.Propability).ToArray();
            var values = group.Select(x => x.Number).ToArray();


            var erg = new D<T, int>(w.automata, values, distribution);
            return erg;

        }

        private static IEnumerable<Tout> Cross<Tin1, Tin2, Tout>(IEnumerable<Tin1> first, IEnumerable<Tin2> seccond, Func<Tin1, Tin2, Tout> f)
        {
            foreach (var t1 in first)
                foreach (var t2 in seccond)
                    yield return f(t1, t2);
        }

        public static implicit operator int(D<T> w)
        {
            if (w is D<T, int>)
                return (int)(w as D<T, int>);
            w.automata.Init(w);
            int role = SelectRole(w);
            w.automata.Role(role);
            return role;
        }

        protected static int SelectRole(D<T> w)
        {
            var posibleRoles = w.automata.PosibleRoles().ToArray();
            int role;
            switch (w.automata.Configuration.DiceResolution)
            {
                case DiceResolution.Random:
                    role = posibleRoles[w.automata.Random.Next(posibleRoles.Length)];
                    break;
                case DiceResolution.SmallestDiceFirst:
                    role = posibleRoles[0];
                    break;
                case DiceResolution.LargestDiceFirst:
                    role = posibleRoles[posibleRoles.Length - 1];
                    break;
                default:
                    throw new NotSupportedException(w.automata.Configuration.DiceResolution.ToString());
            }
            return role;
        }
    }
}
