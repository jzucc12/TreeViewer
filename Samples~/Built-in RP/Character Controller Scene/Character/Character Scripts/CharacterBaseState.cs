namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base state specific to the character
    /// </summary>
    public class CharacterBaseState : BaseState
    {
        protected CharacterStateMachine csm => (CharacterStateMachine)hsm;
        public CharacterBaseState(HSM hsm, string stateName) : base(hsm, stateName)
        {
        }
    }
}
