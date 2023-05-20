namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Add ons to a state to give them additional functionality
    /// </summary>
    public abstract class StateDecorator
    {
        protected HSM hsm;
        public bool onlyOne { get; protected set; }
        
        public StateDecorator(HSM hsm)
        {
            this.hsm = hsm;
        }

        public abstract void Enter();
        public abstract void Tick();
    }
}
