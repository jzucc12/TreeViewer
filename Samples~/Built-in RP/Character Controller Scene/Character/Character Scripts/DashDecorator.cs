using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Allows the player to dash in this state
    /// </summary>
    public class DashDecorator : StateDecorator
    {
        public DashDecorator(HSM hsm) : base(hsm)
        {
            onlyOne = true;
        }

        public override void Enter()
        {
        }

        public override void Tick()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                hsm.InvokeStateEvent(StateEvent.dash);
            }
        }
    }
}
