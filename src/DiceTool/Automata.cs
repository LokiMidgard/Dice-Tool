using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dice
{

    internal class WAutomata<T, P1, P2, P3, P4, P5> : WAutomataBase<T>
    {
        private readonly Dictionary<Tuple<T, P1, P2, P3, P4, P5>, double> resultPropabilitys = new Dictionary<Tuple<T, P1, P2, P3, P4, P5>, double>();

        protected override int NumberOfParameters { get; } = 5;
        public IList<ResultEntry<T, P1, P2, P3, P4, P5>> GetDistribution() => resultPropabilitys.Select(x => new ResultEntry<T, P1, P2, P3, P4, P5>(x.Value, x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Key.Item5, x.Key.Item6)).ToArray();

        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }
        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            var p1 = GetNodeAt<P1>(0, endnode);
            var p2 = GetNodeAt<P2>(1, endnode);
            var p3 = GetNodeAt<P3>(2, endnode);
            var p4 = GetNodeAt<P4>(3, endnode);
            var p5 = GetNodeAt<P5>(4, endnode);
            var tuple = Tuple.Create(endnode.Result, p1, p2, p3, p4, p5);
            if (!resultPropabilitys.ContainsKey(tuple))
                resultPropabilitys.Add(tuple, 0.0);
            resultPropabilitys[tuple] += propability;
        }
    }
    internal class WAutomata<T, P1, P2, P3, P4> : WAutomataBase<T>
    {
        private readonly Dictionary<Tuple<T, P1, P2, P3, P4>, double> resultPropabilitys = new Dictionary<Tuple<T, P1, P2, P3, P4>, double>();

        protected override int NumberOfParameters { get; } = 4;
        public IList<ResultEntry<T, P1, P2, P3, P4>> GetDistribution() => resultPropabilitys.Select(x => new ResultEntry<T, P1, P2, P3, P4>(x.Value, x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Key.Item4, x.Key.Item5)).ToArray();

        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }
        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            var p1 = GetNodeAt<P1>(0, endnode);
            var p2 = GetNodeAt<P2>(1, endnode);
            var p3 = GetNodeAt<P3>(2, endnode);
            var p4 = GetNodeAt<P4>(3, endnode);
            var tuple = Tuple.Create(endnode.Result, p1, p2, p3, p4);
            if (!resultPropabilitys.ContainsKey(tuple))
                resultPropabilitys.Add(tuple, 0.0);
            resultPropabilitys[tuple] += propability;
        }
    }
    internal class WAutomata<T, P1, P2, P3> : WAutomataBase<T>
    {
        private readonly Dictionary<Tuple<T, P1, P2, P3>, double> resultPropabilitys = new Dictionary<Tuple<T, P1, P2, P3>, double>();

        protected override int NumberOfParameters { get; } = 3;
        public IList<ResultEntry<T, P1, P2, P3>> GetDistribution()
        => resultPropabilitys.Select(x => new ResultEntry<T, P1, P2, P3>(x.Value, x.Key.Item1, x.Key.Item2, x.Key.Item3, x.Key.Item4)).ToArray();

        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }
        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            var p1 = GetNodeAt<P1>(0, endnode);
            var p2 = GetNodeAt<P2>(1, endnode);
            var p3 = GetNodeAt<P3>(2, endnode);
            var tuple = Tuple.Create(endnode.Result, p1, p2, p3);
            if (!resultPropabilitys.ContainsKey(tuple))
                resultPropabilitys.Add(tuple, 0.0);
            resultPropabilitys[tuple] += propability;
        }
    }
    internal class WAutomata<T, P1, P2> : WAutomataBase<T>
    {
        private readonly Dictionary<Tuple<T, P1, P2>, double> resultPropabilitys = new Dictionary<Tuple<T, P1, P2>, double>();
        protected override int NumberOfParameters { get; } = 2;
        public IList<ResultEntry<T, P1, P2>> GetDistribution() => resultPropabilitys.Select(x => new ResultEntry<T, P1, P2>(x.Value, x.Key.Item1, x.Key.Item2, x.Key.Item3)).ToArray();

        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }
        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            var p1 = GetNodeAt<P1>(0, endnode);
            var p2 = GetNodeAt<P2>(1, endnode);
            var tuple = Tuple.Create(endnode.Result, p1, p2);
            if (!resultPropabilitys.ContainsKey(tuple))
                resultPropabilitys.Add(tuple, 0.0);
            resultPropabilitys[tuple] += propability;
        }
    }
    internal class WAutomata<T, P1> : WAutomataBase<T>
    {
        private readonly Dictionary<Tuple<T, P1>, double> resultPropabilitys = new Dictionary<Tuple<T, P1>, double>();
        protected override int NumberOfParameters { get; } = 1;
        public IList<ResultEntry<T, P1>> GetDistribution() => resultPropabilitys.Select(x => new ResultEntry<T, P1>(x.Value, x.Key.Item1, x.Key.Item2)).ToArray();
        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }
        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            var p1 = GetNodeAt<P1>(0, endnode);
            var tuple = Tuple.Create(endnode.Result, p1);
            if (!resultPropabilitys.ContainsKey(tuple))
                resultPropabilitys.Add(tuple, 0.0);
            resultPropabilitys[tuple] += propability;
        }
    }
    internal class WAutomata<T> : WAutomataBase<T>
    {
        private readonly Dictionary<T, double> resultPropabilitys = new Dictionary<T, double>();
        protected override int NumberOfParameters { get; } = 0;

        public IList<ResultEntry<T>> GetDistribution() => resultPropabilitys.Select(x => new ResultEntry<T>(x.Value, x.Key)).ToArray();

        public override void Clear()
        {
            base.Clear();
            resultPropabilitys.Clear();
        }

        protected override void AddPropability(EndNode<T> endnode, double propability)
        {
            if (!resultPropabilitys.ContainsKey(endnode.Result))
                resultPropabilitys.Add(endnode.Result, 0.0);
            resultPropabilitys[endnode.Result] += propability;
        }
    }
    abstract internal class WAutomataBase<T>
    {
        internal bool RandomDices { get; } = false;
        protected Node<T> root;
        protected int lastRole;
        protected Node<T> currentNode;

        public int CalculatedPosibilitys { get; set; }

        protected abstract int NumberOfParameters { get; }

        protected abstract void AddPropability(EndNode<T> endnode, double propability);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if next role is posible. false if there is at least one posible dice combination that was not testet</returns>
        public bool NextRoll(T result)
        {
            var endNode = new EndNode<T>(result, currentNode);
            currentNode.Childs[lastRole - 1] = endNode;
            currentNode = null;
            var propability = CalculatePropabilityFactor(endNode);
            AddPropability(endNode, propability);
            endNode.SetFinished();
            CalculatedPosibilitys++;
            return !root.IsFinished;
        }




        protected double CalculatePropabilityFactor(EndNode<T> x)
        {
            var startingDepth = NumberOfParameters;
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

        protected List<Node<T>> getNodeCach;
        protected TParam GetNodeAt<TParam>(int index, EndNode<T> x)
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



        public virtual void Clear()
        {
            currentNode = null;
            root = null;
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

        public DiceCalculatorConfiguration configuration = new DiceCalculatorConfiguration();
        public DiceCalculatorConfiguration Configuration
        {
            get
            {
                return configuration;
            }
            internal set
            {
                configuration = value ?? new DiceCalculatorConfiguration();
            }
        }

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
