using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// UI State for toggleable objects
    /// </summary>
    public class UIToggleState : UIBaseState
    {
        private bool toggleOn;
        public UIBaseState onState { get; private set; }
        public UIBaseState offState { get; private set; }

        public UIToggleState(string nodeName, Toggle toggle, string offStateName, string onStateName) : base(nodeName)
        {
            CreateChildren(offStateName, onStateName);
            toggleOn = toggle.isOn;
            toggle.onValueChanged.AddListener(ToggleChange);
        }

        public UIToggleState(string nodeName, ToggleButton button, string offStateName, string onStateName) : base(nodeName)
        {
            CreateChildren(offStateName, onStateName);
            toggleOn = button.isOn;
            button.onToggle += ToggleChange;
        }

        private void CreateChildren(string offStateName, string onStateName)
        {
            offState = new UIBaseState(offStateName);
            onState = new UIBaseState(onStateName);
            AddChild(offState);
            AddChild(onState);
        }

        private void ToggleChange(bool newValue)
        {
            toggleOn = newValue;
            OnEnter();
        }

        /// <summary>
        /// Only enters the child state that aligns with the toggle state
        /// </summary>
        protected override void OnEnter()
        {
            if(toggleOn)
            {
                offState.ExitState();
                onState.EnterState();
            }
            else
            {
                offState.EnterState();
                onState.ExitState();
            }
        }
    }
}
