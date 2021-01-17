using Dice.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dice.States
{
    class ChoiseManager
    {
        Choise root;
        Choise current;
        private int selectedChoise = -1;
        private bool readOnlyState;
        private List<int> takenChoises = new List<int>();

        public bool IsCompleted => this.root.IsCompleted;

        public double SolvedPropability => this.root.Propability;

        public ChoiseManager()
        {
            //root = new Choise(default, false);
            this.current = this.root;
        }

        public IDisposable EnableMutation()
        {
            this.readOnlyState = false;
            return new DisposeDelegate(() => this.readOnlyState = true);
        }

        public int GetChoise(int depth)
        {
            if (depth < this.takenChoises.Count)
            {
                return this.takenChoises[depth];
            }

            if (this.readOnlyState)
                throw new InvalidOperationException();


            if (this.selectedChoise == -1)
            {
                if (this.root == default)
                    this.root = new Choise(default);
                this.current = this.root;
            }
            else
            {
                if (this.current[this.selectedChoise] == default)
                    this.current[this.selectedChoise] = new Choise(this.current[this.selectedChoise]);

                this.current = this.current[this.selectedChoise];
            }
            this.selectedChoise = this.GetNextChoise(this.current);
            this.takenChoises.Add(this.selectedChoise);
            return this.selectedChoise;
        }

        private int GetNextChoise(in Choise choise)
        {
            return (choise[0].NumberOfLeaf <= choise[1].NumberOfLeaf)
                && !choise[0].IsCompleted
                    ? 0
                    : 1;
        }

        public void Terminate(double propability)
        {
            if (this.selectedChoise == -1)
                this.root = new Choise(default, propability);
            else
                this.current[this.selectedChoise] = new Choise(this.current[this.selectedChoise], propability);
            this.current = this.root;
            this.selectedChoise = -1;
            this.takenChoises = new List<int>(); // dont clear otherwise old cache values will be corupted.
        }

        internal PathToGo CacheKey(int depth)
        {
            return new PathToGo(this.takenChoises, depth);
        }

        internal readonly struct PathToGo : IList<int>, IEquatable<PathToGo>
        {
            private readonly List<int> store;
            private readonly int depth;

            public int Count => this.store.Count - this.depth;


            public int this[int index] => this.store[index + this.depth];

            public PathToGo(List<int> store, int depth)
            {
                this.store = store;
                this.depth = depth;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator(this);
            }

            private struct Enumerator : IEnumerator<int>
            {
                private readonly PathToGo parent;
                private readonly int count;
                private int current;

                public Enumerator(PathToGo parent)
                {
                    this.parent = parent;
                    this.count = parent.Count;
                    this.current = -1;
                }

                public int Current => this.parent[this.current];

                object IEnumerator.Current => this.Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    this.current++;
                    return this.current < this.count;
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }

            int IList<int>.IndexOf(int item)
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i] == item)
                        return i;

                return -1;
            }

            void IList<int>.Insert(int index, int item) => throw new NotSupportedException();

            void IList<int>.RemoveAt(int index) => throw new NotSupportedException();

            bool ICollection<int>.IsReadOnly => true;
            void ICollection<int>.Add(int item) => throw new NotSupportedException();

            void ICollection<int>.Clear() => throw new NotSupportedException();
            bool ICollection<int>.Remove(int item) => throw new NotSupportedException();
            int IList<int>.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

            bool ICollection<int>.Contains(int item) => (this as IList<int>).IndexOf(item) != -1;

            void ICollection<int>.CopyTo(int[] array, int arrayIndex)
            {
                for (int i = 0; i < this.Count; i++)
                    array[i + arrayIndex] = this[i];
            }



            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public override bool Equals(object obj)
            {
                return obj is PathToGo && this.Equals((PathToGo)obj);
            }

            public bool Equals(PathToGo other)
            {
                return this.Count == other.Count
                    && this.SequenceEqual(other);
            }

            public override int GetHashCode()
            {
                var hashCode = new HashCode();

                hashCode.Add(this.Count);
                foreach (var item in this)
                    hashCode.Add(item);

                return hashCode.ToHashCode();
            }

            public static bool operator ==(PathToGo go1, PathToGo go2)
            {
                return go1.Equals(go2);
            }

            public static bool operator !=(PathToGo go1, PathToGo go2)
            {
                return !(go1 == go2);
            }
        }

    }
    readonly struct Choise : IEquatable<Choise>
    {
        readonly Choise[] choises;
        readonly Choise[] parent;
        private readonly bool isLeaf;
        private readonly Guid id; // For comparision
        private readonly double propability;

        public Choise this[int index]
        {
            get { return this.choises[index]; }
            set { this.choises[index] = value; }
        }

        public bool IsCompleted => this.isLeaf || (this.choises?[0].IsCompleted ?? false) && (this.choises?[1].IsCompleted ?? false);

        public int NumberOfLeaf => this.isLeaf ? 1 : (this.choises?[0].NumberOfLeaf ?? 0) + (this.choises?[1].NumberOfLeaf ?? 0);
        public double Propability => this.isLeaf ? this.propability : (this.choises?[0].Propability ?? 0.0) + (this.choises?[1].Propability ?? 0.0);

        public Choise(Choise parent)
        {
            this.isLeaf = false;
            this.parent = new[] { parent };
            this.choises = new Choise[2];
            this.id = Guid.NewGuid();
            this.propability = 0;
        }
        public Choise(Choise parent, double propability)
        {
            this.isLeaf = true;
            this.parent = new[] { parent };
            this.choises = new Choise[0];
            this.id = Guid.NewGuid();
            this.propability = propability;
        }

        public override bool Equals(object obj)
        {
            return obj is Choise && this.Equals((Choise)obj);
        }

        public bool Equals(Choise other)
        {
            return this.id.Equals(other.id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.id);
        }

        public static bool operator ==(Choise choise1, Choise choise2)
        {
            return choise1.Equals(choise2);
        }

        public static bool operator !=(Choise choise1, Choise choise2)
        {
            return !(choise1 == choise2);
        }
    }
}
