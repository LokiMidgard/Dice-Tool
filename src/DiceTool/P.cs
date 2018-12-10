using System;
using System.Collections.Generic;

namespace Dice
{

    internal interface IP
    {
        uint Id { get; }
        IComposer Composer { get; }
    }

    internal static class P
    {
        internal static P<T> Create<T>(IComposer composer, uint id) => new P<T>(composer, id);
    }


    public readonly struct P<T> : IP, IEquatable<P<T>>
    {
        internal readonly uint Id;
        internal readonly IComposer Composer;

        internal P(IComposer composer, uint id)
        {
            this.Id = id;
            this.Composer = composer ?? throw new ArgumentNullException(nameof(composer));
        }

        internal static P<T> Empty => default;

        uint IP.Id => this.Id;

        IComposer IP.Composer => this.Composer;


        public override bool Equals(object obj)
        {
            return obj is P<T> && this.Equals((P<T>)obj);
        }

        public bool Equals(P<T> other)
        {
            return this.Id == other.Id &&
                   EqualityComparer<IComposer>.Default.Equals(this.Composer, other.Composer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id, this.Composer);
        }

        public static bool operator ==(P<T> p1, P<T> p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(P<T> p1, P<T> p2)
        {
            return !(p1 == p2);
        }

        //public static implicit operator P<T>(T e1) => throw new NotImplementedException();//new FixedE<T>((e1, 1));

    }
}
