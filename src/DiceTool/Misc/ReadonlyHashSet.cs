using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Dice
{

    internal class ReadOnlyHashSet<TElement> :
        ISet<TElement>,
        ICollection<TElement>,
        IEnumerable<TElement>,
        IEnumerable
    {
        private readonly HashSet<TElement> original;

        public ReadOnlyHashSet(HashSet<TElement> original)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
        }

        public int Count
        {
            get { return this.original.Count; }
        }

        bool ICollection<TElement>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<TElement>.Add(TElement item)
        {
            ThrowReadOnly();
        }

        void ICollection<TElement>.Clear()
        {
            ThrowReadOnly();
        }

        public bool Contains(TElement item)
        {
            return this.original.Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            this.original.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return this.original.GetEnumerator();
        }

        bool ICollection<TElement>.Remove(TElement item)
        {
            return ThrowReadOnly<bool>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.original.GetEnumerator();
        }

        private static T ThrowReadOnly<T>() { ThrowReadOnly(); return default!; }

        private static void ThrowReadOnly() => throw new NotSupportedException($"{typeof(ReadOnlyHashSet<TElement>)} is Read Only.");

        public bool Add(TElement item)
        {
            return ThrowReadOnly<bool>();
        }

        public void ExceptWith(IEnumerable<TElement> other)
        {
            ThrowReadOnly();
        }

        public void IntersectWith(IEnumerable<TElement> other)
        {
            ThrowReadOnly();
        }

        public bool IsProperSubsetOf(IEnumerable<TElement> other)
        {
            return this.original.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<TElement> other)
        {
            return this.original.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<TElement> other)
        {
            return this.original.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<TElement> other)
        {
            return this.original.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<TElement> other)
        {
            return this.original.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<TElement> other)
        {
            return this.original.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<TElement> other)
        {
            ThrowReadOnly();
        }

        public void UnionWith(IEnumerable<TElement> other)
        {
            ThrowReadOnly();
        }
    }

    internal static class ReadOnlyHashSetExtension
    {
        public static ReadOnlyHashSet<TElement> AsReadOnly<TElement>(this HashSet<TElement> set)
        {
            return new ReadOnlyHashSet<TElement>(set);
        }
    }
}
