using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dice.Misc
{
    class ArrayEqualityComparer<T> : EqualityComparer<T[]>
    {
        public override bool Equals(T[]? x, T[]? y)
        {
            if (x?.Length != y?.Length)
                return false;
            if (y is null)
                return false;
            return x?.SequenceEqual(y) ?? false;
        }

        public override int GetHashCode(T[] obj)
        {
            var h = new HashCode();
            h.Add(obj.Length);

            for (int i = 0; i < obj.Length; i++)
                h.Add(obj[i]);
            return h.ToHashCode();
        }

    }
}
