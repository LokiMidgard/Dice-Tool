using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{

    internal class WAutomata<T>
    {
        private Node<T> root;
        private int lastRole;
        private Node<T> currentNode;
        private readonly List<EndNode<T>> endNodeCach = new List<EndNode<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if next role is posible. false if there is at least one posible dice combination that was not testet</returns>
        public bool NextRoll(T result)
        {
            var endNode = new EndNode<T>(result, currentNode);
            endNodeCach.Add(endNode);
            currentNode.Childs[lastRole - 1] = endNode;
            currentNode = null;
            endNode.SetFinished();
            return !root.IsFinished;
        }


        public IList<ResultEntry<T>> GetDistribution()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T>(
                CalculatePropabilityFactor(x, 0),
                x.Result)
            ).ToArray();
        }
        public IList<ResultEntry<T, P1>> GetDistribution<P1>()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T, P1>(
                CalculatePropabilityFactor(x, 1),
                x.Result,
                GetNodeAt<P1>(0, x)
            )).ToArray();
        }

        public IList<ResultEntry<T, P1, P2>> GetDistribution<P1, P2>()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T, P1, P2>(
                CalculatePropabilityFactor(x, 2),
                x.Result,
                GetNodeAt<P1>(0, x),
                GetNodeAt<P2>(1, x)
            )).ToArray();
        }

        public IList<ResultEntry<T, P1, P2, P3>> GetDistribution<P1, P2, P3>()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T, P1, P2, P3>(
                CalculatePropabilityFactor(x, 3),
                x.Result,
                GetNodeAt<P1>(0, x),
                GetNodeAt<P2>(1, x),
                GetNodeAt<P3>(2, x)
            )).ToArray();
        }

        public IList<ResultEntry<T, P1, P2, P3, P4>> GetDistribution<P1, P2, P3, P4>()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T, P1, P2, P3, P4>(
                CalculatePropabilityFactor(x, 4),
                x.Result,
                GetNodeAt<P1>(0, x),
                GetNodeAt<P2>(1, x),
                GetNodeAt<P3>(2, x),
                GetNodeAt<P4>(3, x)
            )).ToArray();
        }
        public IList<ResultEntry<T, P1, P2, P3, P4, P5>> GetDistribution<P1, P2, P3, P4, P5>()
        {
            return endNodeCach.Select(x =>
            new ResultEntry<T, P1, P2, P3, P4, P5>(
                CalculatePropabilityFactor(x, 5),
                x.Result,
                GetNodeAt<P1>(0, x),
                GetNodeAt<P2>(1, x),
                GetNodeAt<P3>(2, x),
                GetNodeAt<P4>(3, x),
                GetNodeAt<P5>(4, x)
            )).ToArray();
        }
        public int GetCalculatedPosibilitys()

        {
            return endNodeCach.Count;
        }

        private double CalculatePropabilityFactor(EndNode<T> x, int startingDepth)
        {
            var count = 1.0;
            for (Node<T> current = x, last = null; current != null && current.Depth >= startingDepth; last = current, current = current.Parent)
            {
                if (current is EndNode<T>)
                    continue;
                var nextNodeIndex = current.Childs.IndexOf(last);
                count *= current.Dice.Propabilitys[nextNodeIndex];
            }
            return count;
        }

        private List<Node<T>> getNodeCach;
        private TParam GetNodeAt<TParam>(int index, EndNode<T> x)
        {
            var list = getNodeCach;
            if (list == null || list[list.Count - 1] != x)
            {
                list = new List<Node<T>>();

                for (var current = x as Node<T>; current != null; current = current.Parent)
                    list.Add(current);
                list.Reverse();
                getNodeCach = list;
            }
            var first = list[index];
            var seccond = list[index + 1];
            var selected = (first.Childs as IList<Node<T>>).IndexOf(seccond);
            var parameters = (first.Dice as D<T, TParam>).Parameters;
            return parameters[selected];
        }



        public void Clear()
        {
            currentNode = null;
            root = null;
            endNodeCach.Clear();
        }

        internal void Role(int i)
        {
            lastRole = i;
            if (i > currentNode.DiceSize)
                throw new ArgumentOutOfRangeException("Würfel ergebnis höher als Seitenzahl des Würfels");
            if (i <= 0)
                throw new ArgumentOutOfRangeException("Würfel ergebnis war 0 oder kleiner.");
            if (currentNode.Childs[i - 1]?.IsFinished ?? false)
                throw new InvalidOperationException("Dieser Möglichkeitspfad wurde bereits komplet abgearbeitet!");
        }


        internal int CurrentDiceSize => currentNode.DiceSize;

        internal IEnumerable<int> PosibleRoles()
        {
            return currentNode.Childs.Select((node, index) => new { index, node }).Where(x => !(x.node?.IsFinished ?? false)).Select(x => x.index + 1);
        }

        internal void Init(D<T> dice)
        {
            if (currentNode == null)
                if (root == null)
                    root = currentNode = new Node<T>(dice, null);
                else if (root.DiceSize != dice.Size)
                    throw new InvalidOperationException("Forherige durchgänge starteten mit anderem Würfel.");
                else
                    currentNode = root;
            else if (currentNode.Childs[lastRole - 1] != null)
                if (currentNode.Childs[lastRole - 1].DiceSize != dice.Size)
                    throw new InvalidOperationException("Forherige durchgänge nutzten anderen würfel an dierser stelle. Das Programm muss deterministisch sein in abhänigkeit der würfelergebnisse.");
                else
                    currentNode = currentNode.Childs[lastRole - 1];
            else
                currentNode = currentNode.Childs[lastRole - 1] = new Node<T>(dice, currentNode);


        }

    }

}
