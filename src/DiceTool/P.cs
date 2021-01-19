using System;
using System.Collections.Generic;

namespace Dice
{

    internal interface IP
    {
        string Id { get; }
        IComposer Composer { get; }

        Type Type { get; }
    }

    internal static class P
    {
        internal static P<T> Create<T>(IComposer composer, string id) => new P<T>(composer, id);
    }


    [System.Diagnostics.DebuggerDisplay("{Id}")]
    public readonly struct P<T> : IP, IEquatable<P<T>>
    {
        internal readonly string Id;
        internal readonly IComposer Composer;

        internal P(IComposer composer, string id)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Composer = composer ?? throw new ArgumentNullException(nameof(composer));
        }

        internal static P<T> Empty => default;

        string IP.Id => this.Id;

        IComposer IP.Composer => this.Composer;

        Type IP.Type => typeof(T);

        public override bool Equals(object? obj)
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
