using System;
using System.Collections.Generic;

namespace Dice
{

    internal interface IP
    {
        uint Id { get; }
        IComposer Composer { get; }
    }

    
    public struct P<T> : IP, IEquatable<P<T>>
    {
        internal readonly uint Id;
        internal readonly IComposer Composer;

        private P(IComposer composer)
        {
            this.Id = composer.CreateId();
            this.Composer = composer ?? throw new ArgumentNullException(nameof(composer));
        }

        internal static P<T> Empty => default;

        uint IP.Id => this.Id;

        IComposer IP.Composer => this.Composer;

        internal static P<T> Create(IComposer composer) => new P<T>(composer);

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
