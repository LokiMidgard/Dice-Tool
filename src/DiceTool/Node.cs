using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{

    internal class Node<T>
    {
        public Node(D<T> count, Node<T> parent)
        {
            Childs = new Node<T>[count?.Size ?? 0];
            this.Parent = parent;
            this.Dice = count;
            Depth = Parent?.Depth + 1 ?? 0;
        }
        public IList<Node<T>> Childs { get; private set; }
        public D<T> Dice { get; }

        public int Depth { get; }

        /// <summary>
        /// Gibt an wie gewichtig dieser weg ist. Je mehr kombinationen dieser wert ergibt desto höher die gewichting.
        /// </summary>


        public int DiceSize => Childs.Count;

        private bool isFinished;
        //public virtual bool IsFinished => isFinished = isFinished || Childs.All(x => x?.IsFinished ?? false);
        //private bool isFinished;
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
            protected set
            {
                if (value)
                {
                    isFinished = value;
                    var alFinished = Parent?.Childs.All(x => x?.IsFinished ?? false) ?? false;
                    if (alFinished)
                        Parent.IsFinished = true;
                    this.Childs = null; // clear memory
                }
                else if (isFinished)
                {
                    throw new InvalidOperationException("Einmal beendet sollte immer beendet sein!");
                }
            }
        }

        public Node<T> Parent { get; }
    }
}
