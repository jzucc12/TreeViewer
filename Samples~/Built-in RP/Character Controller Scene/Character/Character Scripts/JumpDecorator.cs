using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Allows the player to jump in this state
    /// </summary>
    public class JumpDecorator : StateDecorator
    {
        public JumpDecorator(HSM hsm) : base(hsm)
        {
            onlyOne = true;
        }

        public override void Enter()
        {
        }

        public override void Tick()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                var csm = (CharacterStateMachine)hsm;
                csm.Jump();
                csm.InvokeStateEvent(StateEvent.jump);
            }
        }
    }
}
