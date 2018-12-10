using Dice.States;
using System;

namespace Dice
{
    public static class PExtensions
    {

        public static P<bool> Not(this in P<bool> e)
        {
            var composer = e.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => !x1));
            return p;
        }
        public static P<bool> And(this in P<bool> e1, in P<bool> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 && x2));
            return p;
        }

        [Obsolete("Not Yet implemented", true)]
        public static P<bool> And(this in P<bool> e1, params P<bool>[] e) => throw new NotImplementedException();
        public static P<bool> Or(this in P<bool> e1, in P<bool> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 || x2));
            return p;
        }

        [Obsolete("Not Yet implemented", true)]
        public static P<bool> Or(this in P<bool> e1, params P<bool>[] e) => throw new NotImplementedException();


        public static P<int> BitNegate(this in P<int> e)
        {
            var composer = e.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => ~x1));
            return p;
        }
        public static P<uint> BitNegate(this in P<uint> e)
        {
            var composer = e.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => ~x1));
            return p;
        }
        public static P<long> BitNegate(this in P<long> e)
        {
            var composer = e.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => ~x1));
            return p;
        }
        public static P<ulong> BitNegate(this in P<ulong> e)
        {
            var composer = e.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => ~x1));
            return p;
        }




        public static P<int> Increment(this ref P<int> e)
        {
            var composer = e.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }
        public static P<uint> Increment(this ref P<uint> e)
        {
            var composer = e.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }
        public static P<long> Increment(this ref P<long> e)
        {
            var composer = e.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }
        public static P<ulong> Increment(this ref P<ulong> e)
        {
            var composer = e.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }
        public static P<float> Increment(this ref P<float> e)
        {
            var composer = e.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }
        public static P<double> Increment(this ref P<double> e)
        {
            var composer = e.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 + 1));
            e = p;
            return p;
        }




        public static P<int> Decrement(this ref P<int> e)
        {
            var composer = e.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }
        public static P<uint> Decrement(this ref P<uint> e)
        {
            var composer = e.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }
        public static P<long> Decrement(this ref P<long> e)
        {
            var composer = e.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }
        public static P<ulong> Decrement(this ref P<ulong> e)
        {
            var composer = e.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }
        public static P<float> Decrement(this ref P<float> e)
        {
            var composer = e.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }
        public static P<double> Decrement(this ref P<double> e)
        {
            var composer = e.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(TransformState.Create(composer.CurrentState, p, e, (x1) => x1 - 1));
            e = p;
            return p;
        }



        public static P<int> Add(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }
        public static P<uint> Add(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }
        public static P<long> Add(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }
        public static P<ulong> Add(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }
        public static P<float> Add(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }
        public static P<double> Add(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 + x2));
            return p;
        }



        public static P<int> Substract(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }
        public static P<uint> Substract(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }
        public static P<long> Substract(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }
        public static P<ulong> Substract(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }
        public static P<float> Substract(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }
        public static P<double> Substract(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 - x2));
            return p;
        }



        public static P<int> Multiply(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }
        public static P<uint> Multiply(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }
        public static P<long> Multiply(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }
        public static P<ulong> Multiply(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }
        public static P<float> Multiply(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }
        public static P<double> Multiply(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 * x2));
            return p;
        }



        public static P<int> Divide(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }
        public static P<uint> Divide(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }
        public static P<long> Divide(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }
        public static P<ulong> Divide(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }
        public static P<float> Divide(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }
        public static P<double> Divide(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 / x2));
            return p;
        }



        public static P<int> BitAnd(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 & x2));
            return p;
        }
        public static P<uint> BitAnd(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 & x2));
            return p;
        }
        public static P<long> BitAnd(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 & x2));
            return p;
        }
        public static P<ulong> BitAnd(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 & x2));
            return p;
        }
        // this does not work (yet?)
        //public static P<T> BitAnd<T>(this in P<T> e1, in P<T> e2) where T : Enum
        //{
        //    var composer = e1.Composer;
        //    var p = P<T>.Create(composer);
        //    composer.NextStates( CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 & x2));
        //    return p;
        //}


        public static P<int> BitOr(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 | x2));
            return p;
        }
        public static P<uint> BitOr(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 | x2));
            return p;
        }
        public static P<long> BitOr(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 | x2));
            return p;
        }
        public static P<ulong> BitOr(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 | x2));
            return p;
        }


        public static P<int> BitXOr(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 ^ x2));
            return p;
        }
        public static P<uint> BitXOr(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 ^ x2));
            return p;
        }
        public static P<long> BitXOr(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 ^ x2));
            return p;
        }
        public static P<ulong> BitXOr(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 ^ x2));
            return p;
        }


        public static P<int> Modulo(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }
        public static P<uint> Modulo(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }
        public static P<long> Modulo(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }
        public static P<ulong> Modulo(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }
        public static P<float> Modulo(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<float>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }
        public static P<double> Modulo(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<double>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 % x2));
            return p;
        }


        public static P<int> LeftShift(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 << x2));
            return p;
        }
        public static P<uint> LeftShift(this in P<uint> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 << x2));
            return p;
        }
        public static P<long> LeftShift(this in P<long> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 << x2));
            return p;
        }
        public static P<ulong> LeftShift(this in P<ulong> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 << x2));
            return p;
        }



        public static P<int> RightShift(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<int>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >> x2));
            return p;
        }
        public static P<uint> RightShift(this in P<uint> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<uint>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >> x2));
            return p;
        }
        public static P<long> RightShift(this in P<long> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<long>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >> x2));
            return p;
        }
        public static P<ulong> RightShift(this in P<ulong> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<ulong>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >> x2));
            return p;
        }



        public static P<bool> AreEqual<T>(this in P<T> e1, in P<T> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => Equals(x1, x2))); 
            return p;
        }

        public static P<bool> LessThen(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<byte> e1, in P<byte> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<short> e1, in P<short> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<ushort> e1, in P<ushort> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }
        public static P<bool> LessThen(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 < x2));
            return p;
        }


        public static P<bool> GreaterThen(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<byte> e1, in P<byte> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<short> e1, in P<short> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<ushort> e1, in P<ushort> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }
        public static P<bool> GreaterThen(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 > x2));
            return p;
        }


        public static P<bool> LessOrEqual(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<byte> e1, in P<byte> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<short> e1, in P<short> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<ushort> e1, in P<ushort> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }
        public static P<bool> LessOrEqual(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 <= x2));
            return p;
        }



        public static P<bool> GreaterOrEqual(this in P<int> e1, in P<int> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<uint> e1, in P<uint> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<long> e1, in P<long> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<ulong> e1, in P<ulong> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<byte> e1, in P<byte> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<short> e1, in P<short> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<ushort> e1, in P<ushort> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<float> e1, in P<float> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }
        public static P<bool> GreaterOrEqual(this in P<double> e1, in P<double> e2)
        {
            var composer = e1.Composer;
            var p = P<bool>.Create(composer);
            composer.NextStates(CombinationState.Create(composer.CurrentState, p, e1, e2, (x1, x2) => x1 >= x2));
            return p;
        }


        public static P<TOut> Select<TOut, TIn>(this in P<TIn> e, Func<TIn, TOut> selector)
        {
            var composer = e.Composer;
            var p = P<TOut>.Create(composer);
            composer.NextStates(new TransformState<TIn, TOut>(composer.CurrentState, p, e, selector));
            return p;
        }

        public static PostThen Then(this in P<bool> condition, Action then)
        {
            var composer = condition.Composer;
            var (thenState, elseState) = DevideState.Create(composer.CurrentState, condition);
            var selectedState = composer.NextStates(thenState, elseState);
            if (selectedState == thenState)
                then();

            return new PostThen(elseState);


        }

        public static PostThen ElseIf(this in PostThen postThen, P<bool> condition, Action then)
        {
            if (postThen.ShouldExecute)
            {
                var composer = condition.Composer;
                var (thenState, elseState) = DevideState.Create(composer.CurrentState, condition);
                var selectedState = composer.NextStates(thenState, elseState);
                if (selectedState == thenState)
                    then();

                return new PostThen(elseState);
            }
            return default;
        }

        public static void Else(this in PostThen postThen, Action then)
        {
            if (postThen.ShouldExecute)
                then();
        }

        public readonly struct PostThen
        {
            private readonly State? executeWhenState;

            internal bool ShouldExecute => this.executeWhenState != null && this.executeWhenState.Composer.CurrentState == this.executeWhenState;
            internal PostThen(State? executeWhenState)
            {
                this.executeWhenState = executeWhenState;
            }
        }

    }
}
