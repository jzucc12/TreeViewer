namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Player dashing state
    /// </summary>
    public class CharacterDashState : CharacterBaseState
    {
        public CharacterDashState(CharacterStateMachine csm, string stateName, float duration) : base(csm, stateName)
        {
            AddDecorator(new DelayDecorator(csm, duration));
            AddDecorator(new AnimatorDecorator(csm, "Dash"));
        }

        public override void EnterState(bool revert)
        {
            base.EnterState(revert);
            csm.Dash();
        }

        public override void ExitState()
        {
            base.ExitState();
            csm.EndDash();
        }
    }
}
