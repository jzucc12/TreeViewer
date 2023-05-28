using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// UI State for sliders
    /// </summary>
    public class UISliderState : UIBaseState
    {
        private Dictionary<float, UIBaseState> subStates = new Dictionary<float, UIBaseState>();
        private UIBaseState zeroState;
        private UIBaseState middleState;
        private UIBaseState oneState;
        private float myValue;
        private UIBaseState activeState;

        /// <summary>
        /// Default use where substates are for the slider ends and the middle
        /// </summary>
        public UISliderState(string nodeName, Slider slider, string zeroStateName, string middleStateName, string oneStateName) : base(nodeName)
        {
            myValue = slider.value;
            slider.onValueChanged.AddListener(ChangeState);
            SetSubstates( new (float, string)[] { (0, zeroStateName), (0.99f, middleStateName), (1, oneStateName) });
        }

        /// <summary>
        /// Custom use where you can define their own breakpoints for creating slider substates
        /// </summary>
        public UISliderState(string nodeName, Slider slider, IEnumerable<(float, string)> breaks) : base(nodeName)
        {
            myValue = slider.value;
            slider.onValueChanged.AddListener(ChangeState);
            SetSubstates(breaks);
        }

        /// <summary>
        /// Creates substates based on the passed in break points.
        /// </summary>
        private void SetSubstates(IEnumerable<(float, string)> breaks)
        {
            foreach(var entry in breaks)
            {
                UIBaseState newState =  new UIBaseState(entry.Item2);
                subStates.Add(entry.Item1, newState);
                AddChild(newState);
            }
        }

        private void ChangeState(float newValue)
        {
            myValue = newValue;
            OnEnter();
        }

        /// <summary>
        /// Enters substate based on slider value if that state isn't active already
        /// </summary>
        protected override void OnEnter()
        {
            //Search the substates for the first state that is lower than the current value
            UIBaseState newState = subStates.First((pair) => myValue <= pair.Key).Value;
         
            //Enter that state if not active already
            if(activeState != newState)
            {
                activeState?.ExitState();
                newState.EnterState();
                activeState = newState;
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            activeState = null;
        }
    }
}
