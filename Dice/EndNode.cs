using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{
    internal class EndNode<T> : Node<T>
    {
        public T Result { get; }

        public EndNode(T result, Node<T> parent) : base(null, parent)
        {
            this.Result = result;
        }

        internal void SetFinished()
        {
            this.IsFinished = true;
        }

    }
}
