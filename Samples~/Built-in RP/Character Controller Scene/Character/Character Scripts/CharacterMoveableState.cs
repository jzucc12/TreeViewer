using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// State that allows the player to move with input
    /// </summary>
    public class CharacterMoveableState : CharacterBaseState
    {
        public CharacterMoveableState(CharacterStateMachine csm, string stateName) : base(csm, stateName)
        {
        }

        public override void StateTick()
        {
            base.StateTick();
            int moveDir = 0;
            if(Input.GetKey(KeyCode.A))
            {
                moveDir--;
            }
            if(Input.GetKey(KeyCode.D))
            {
                moveDir++;
            }
            csm.SetXDir(moveDir);
        }
    }
}
