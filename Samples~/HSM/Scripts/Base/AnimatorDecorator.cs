namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Sets an animation trigger upon entering
    /// </summary>
    public class AnimatorDecorator : StateDecorator
    {
        string animKey;

        public AnimatorDecorator(HSM hsm, string animKey) : base(hsm)
        {
            this.animKey = animKey;
            onlyOne = true;
        }

        public override void Enter()
        {
            hsm.animator.SetTrigger(animKey);
        }

        public override void Tick() { }
    }
}
