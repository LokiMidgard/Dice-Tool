﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    public class D<T, TParam> : D<T>
    {
        internal D(WAutomata<T> automata, IList<TParam> p1, IList<double> propabilityDistribution = null) : base(p1.Count, automata)
        {
            this.Parameters = p1;
            if (propabilityDistribution != null)
            {
                if (p1.Count != propabilityDistribution.Count)
                    throw new ArgumentException("Number of Parameters and Propabilitys must have the same size.");
                Propabilitys = propabilityDistribution;
            }
        }

        public IList<TParam> Parameters { get; }

        public static implicit operator TParam(D<T, TParam> w)
        {
            w.automata.Init(w);
            var posibleRoles = w.automata.PosibleRoles().ToArray();
            var role = posibleRoles[r.Next(posibleRoles.Length)];
            w.automata.Role(role);
            return w.Parameters[role - 1];
        }


    }



    public class D<T>
    {
        internal static readonly Random r = new Random();
        internal readonly WAutomata<T> automata;

        public IList<double> Propabilitys { get; internal set; }



        public int Size { get; }

        internal D(int size, WAutomata<T> automata)
        {
            this.Size = size;
            this.automata = automata;
            Propabilitys = Enumerable.Repeat(1.0 / size, size).ToArray();
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

            var allVariants = Math.Pow( w.Size, count);

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
            var posibleRoles = w.automata.PosibleRoles().ToArray();
            var role = posibleRoles[r.Next(posibleRoles.Length)];
            w.automata.Role(role);
            return role;
        }

    }
}