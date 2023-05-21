using System.Collections;
using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base state for the character scene
    /// </summary>
    public class BaseState : ITreeNodeViewer, IEnumerable<ITreeNodeViewer>
    {
        protected HSM hsm;
        private string stateName;
        public bool IsActive { get; private set; }
        private bool isDirty = false;

        private BaseState parent;
        private BaseState activeSubState; 
        private BaseState lastActiveSubState;
        private List<BaseState> children = new List<BaseState>();

        private List<StateDecorator> decorators = new List<StateDecorator>();
        private Dictionary<StateEvent, (BaseState, BaseState)> transitions = new Dictionary<StateEvent, (BaseState, BaseState)>();


        #region //Set up
        public BaseState(HSM hsm, string stateName)
        {
            this.stateName = stateName;
            this.hsm = hsm;
        }

        public void AddDecorator(StateDecorator decorator)
        {
            if(decorator.onlyOne && decorators.Find(dec => dec.GetType() == decorator.GetType()) != null)
            {
                throw new System.Exception($"{stateName} can't have duplicate decorators of type: {decorator.GetType()}");
            }
            decorators.Add(decorator);
        }

        public void AddChild(BaseState state)
        {
            children.Add(state);
            state.parent = this;
        }

        public void AddTransition(StateEvent evt, BaseState from, BaseState to)
        {
            transitions.Add(evt, (from, to));
        }

        public void AddRevert(StateEvent evt, BaseState from)
        {
            transitions.Add(evt, (from, null));
        }
        #endregion

        #region //Transitioning
        public virtual void EnterState(bool revert)
        {
            if(revert)
            {
                activeSubState = lastActiveSubState;
            }
            else if(children.Count > 0)
            {
                activeSubState = children[0];
            }
            lastActiveSubState = null;


            foreach(StateDecorator decorator in decorators)
            {
                decorator.Enter();
            }
            hsm.OnStateEvent += TransitionEvent;
            IsActive = true;
            MakeDirty();
            activeSubState?.EnterState(revert);

        }

        public virtual void ExitState()
        {
            activeSubState?.ExitState();
            hsm.OnStateEvent -= TransitionEvent;
            lastActiveSubState = activeSubState;
            activeSubState = null;
            IsActive = false;
            MakeDirty();
        }

        private void MakeDirty()
        {
            isDirty = true;
            parent?.MakeDirty();
        }

        protected void TransitionEvent(StateEvent evt)
        {
            if(transitions.ContainsKey(evt))
            {
                //Only transition if the source state matches
                //the current substate
                var transition = transitions[evt];
                if (transition.Item1 != activeSubState)
                {
                    return;
                }

                //Only revert if their is no destination state
                bool revert = false;
                BaseState nextState = transition.Item2;
                if(nextState == null)
                {
                    revert = true;
                    nextState = lastActiveSubState;
                }

                //Transition
                activeSubState?.ExitState();
                lastActiveSubState = activeSubState;
                activeSubState = nextState;
                activeSubState?.EnterState(revert);
            }
        }
        #endregion

        public virtual void StateTick() 
        { 
            foreach(StateDecorator decorator in decorators)
            {
                decorator.Tick();
            }
            activeSubState?.StateTick();
        }

        #region //Interface specific
        public string GetNodeName()
        {
            return stateName;
        }

        public bool IsChildOf(ITreeNodeViewer state)
        {
            var charState = (BaseState)state;
            return charState.children.Contains(this);
        }

        public bool IsNodeDirty()
        {
            bool dirty = isDirty;
            isDirty = false;
            return dirty;
        }
        #endregion
    
        public IEnumerator<ITreeNodeViewer> GetEnumerator()
        {
            yield return this;
            for (int ii = 0; ii < children.Count; ii++)
            {
                if (children[ii] != null)
                {
                    foreach(ITreeNodeViewer state in children[ii])
                    {
                        yield return state;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}