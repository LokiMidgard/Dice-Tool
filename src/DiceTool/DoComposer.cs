using Dice.States;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Dice
{
    public partial class Composer<TInput>
    {

        private class DoComposer : IComposer
        {
            private enum DoComposerState
            {
                FirstRun,
                SeccondRun
            }


            private readonly IInternalComposer parent;
            private readonly Func<P<bool>> @do;
            private readonly uint startId;
            private uint specialId;
            private uint createdIdsCount;

            public StateAdministrator State { get; }
            public MergeState MergedState { get; private set; }

            private DoComposerState ProcessState;

            public DoState DoState { get; }

            public DoComposer(IInternalComposer parent, System.Func<P<bool>> @do)
            {
                this.parent = parent;
                this.@do = @do;
                this.startId = parent.CreateId();
                this.createdIdsCount = 0;
                this.ProcessState = DoComposerState.FirstRun;
                this.DoState = new DoState(parent.State.CurrentState);

                this.State = new StateAdministrator(this.DoState);
            }

            private readonly List<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)> replacedVariableList = new List<(IP issuedVariable, IP originalVariable, IP whileBodyVariable)>();

            public void Initilize()
            {

                var whiles = new List<WhileState>();
                do
                {
                    var savedState = this.State.ExportCurrentState();
                    this.@do();
                    System.Diagnostics.Debug.Assert(this.createdIdsCount == this.createdIdsList.Count);
                    this.createdIdsCount = 0;
                    this.ProcessState = DoComposerState.SeccondRun;
                    this.State.ImportSavedState(savedState);
                    var condition = this.@do();

                    // Save the replaced variabes
                    var whileState = new WhileState(this.State.CurrentState, condition, this.replacedVariableList.ToArray());
                    this.replacedVariableList.Clear();
                    this.State.NextStates(whileState);

                    whiles.Add(whileState);


                } while (this.State.MoveNext()); // Will propably not work;

                this.MergedState = new MergeState(whiles.Select(x => x.EndState));
                for (int i = 1; i < whiles.Count; i++)
                {
                    var currentState = whiles[i];
                    var previousState = whiles[i - 1];

                    foreach (var item in currentState.VariableMapping)
                        this.MergedState.AddMapping(previousState.EndState, item.issuedVariable, item.originalVariable);

                }


                // When finished we need to put our parent back in control
                this.parent.State.NextStates(this.MergedState);
                this.parent.SetId(this.startId + this.createdIdsCount + this.specialId);
                this.parent.ComposerWraper.Internal = this.parent;
            }

            public P<TOut> CreateCombineState<TIn1, TIn2, TOut>(P<TIn1> e1, P<TIn2> e2, Func<TIn1, TIn2, TOut> func)
            {
                var p = P.Create<TOut>(this, this.CreateId(ref e1, ref e2));
                var state = new CombinationState<TIn1, TIn2, TOut>(this.State.CurrentState, p, e1, e2, func);
                this.State.NextStates(state);
                return p;
            }

            private readonly List<IdHistory> createdIdsList = new List<IdHistory>();


            private readonly struct IdHistory
            {
                public readonly uint IdCreated;
                public readonly IP? FirstParent;
                public readonly IP? SeccondParent;

                public IdHistory(uint idCreated, IP? firstParent, IP? seccondParent)
                {
                    this.IdCreated = idCreated;
                    this.FirstParent = firstParent;
                    this.SeccondParent = seccondParent;
                }
            }

            private uint CreateId()
            {
                var id = this.createdIdsCount + this.startId + 1;
                if (this.ProcessState == DoComposerState.FirstRun)
                {
                    this.createdIdsList.Add(new IdHistory(id, null, null));
                }
                else
                {
                    var lastState = this.createdIdsList[(int)this.createdIdsCount];
                    System.Diagnostics.Debug.Assert(lastState.IdCreated == id);
                    System.Diagnostics.Debug.Assert(lastState.FirstParent is null && lastState.SeccondParent is null);
                }
                this.createdIdsCount++;
                return id;
            }
            private uint CreateId<T1>(ref P<T1> first)
            {
                var id = this.createdIdsCount + this.startId + 1;
                if (this.ProcessState == DoComposerState.FirstRun)
                {
                    this.createdIdsList.Add(new IdHistory(id, first, null));
                }
                else
                {
                    var lastState = this.createdIdsList[(int)this.createdIdsCount];
                    System.Diagnostics.Debug.Assert(lastState.IdCreated == id);
                    System.Diagnostics.Debug.Assert(lastState.FirstParent != null && lastState.SeccondParent is null);
                    System.Diagnostics.Debug.Assert(lastState.FirstParent is P<T1>);
                    if (lastState.FirstParent.Id != first.Id)
                    {
                        this.specialId++;
                        var newP = P.Create<T1>(this.parent.ComposerWraper, (uint)(this.startId + this.specialId + this.createdIdsList.Count));
                        this.AddMapping(newP, (P<T1>)lastState.FirstParent, first);
                        first = newP;
                    }
                }
                this.createdIdsCount++;
                return id;
            }
            private uint CreateId<T1, T2>(ref P<T1> first, ref P<T2> seccond)
            {
                var id = this.createdIdsCount + this.startId + 1;
                if (this.ProcessState == DoComposerState.FirstRun)
                {
                    this.createdIdsList.Add(new IdHistory(id, first, seccond));
                }
                else
                {
                    var lastState = this.createdIdsList[(int)this.createdIdsCount];
                    System.Diagnostics.Debug.Assert(lastState.IdCreated == id);
                    System.Diagnostics.Debug.Assert(lastState.FirstParent != null && lastState.SeccondParent != null);
                    System.Diagnostics.Debug.Assert(lastState.FirstParent is P<T1> && lastState.SeccondParent is P<T2>);
                    if (lastState.FirstParent.Id != first.Id)
                    {
                        this.specialId++;
                        var newP = P.Create<T1>(this.parent.ComposerWraper, (uint)(this.startId + this.specialId + this.createdIdsList.Count));
                        this.AddMapping(newP, (P<T1>)lastState.FirstParent, first);
                        first = newP;
                    }

                    if (lastState.SeccondParent?.Id != seccond.Id)
                    {
                        this.specialId++;
                        var newP = P.Create<T2>(this.parent.ComposerWraper, (uint)(this.startId + this.specialId + this.createdIdsList.Count));
                        this.AddMapping(newP, (P<T2>)lastState.SeccondParent, seccond);
                        seccond = newP;
                    }
                }
                this.createdIdsCount++;
                return id;
            }

            private void AddMapping<T>(P<T> newValue, P<T> replacedOriginal, P<T> replacedNew)
            {
                // when the newValue is asked, and we don't wan't to iterate more loops we search DoState parent for replaceOriginal
                // If we want to perform more loops we will ask the WhileState for replaceNew (which shuld result in us getting asked again for newValue
                throw new NotImplementedException();
            }

            public bool CreateDevideState(P<bool> p)
            {
                var trueState = new DevideState(this.State.CurrentState, p, true);
                var falseState = new DevideState(this.State.CurrentState, p, false);
                return trueState == this.State.NextStates(trueState, falseState);
            }

            public P<T> CreateConstState<T>(T value)
            {
                var p = P.Create<T>(this, this.CreateId());
                var state = new NewConstState<T>(this.State.CurrentState, p, value);
                this.State.NextStates(state);
                return p;

            }

            public P<T> CreateVariableState<T>(params (T value, double propability)[] distribution)
            {
                var p = P.Create<T>(this, this.CreateId());
                var state = new NewVariableState<T>(this.State.CurrentState, p, distribution);
                this.State.NextStates(state);
                return p;
            }

            public P<TOut> CreateTransformState<TIn, TOut>(P<TIn> e, Func<TIn, TOut> func)
            {
                var p = P.Create<TOut>(this, this.CreateId(ref e));
                var state = new TransformState<TIn, TOut>(this.State.CurrentState, p, e, func);
                this.State.NextStates(state);
                return p;
            }



        }
    }
}