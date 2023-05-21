using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Calls a delay event after a specified amount of time
    /// </summary>
    public class DelayDecorator : StateDecorator
    {
        private float delayTime;
        private float currentTime;
        private bool delayed = false;

        public DelayDecorator(HSM hsm, float delayTime) : base(hsm)
        {
            this.delayTime = delayTime;
            onlyOne = false;
        }

        public override void Enter()
        {
            currentTime = 0;
            delayed = false;
        }

        public override void Tick()
        {
            currentTime += Time.deltaTime;
            if(!delayed && currentTime >= delayTime)
            {
                hsm.InvokeStateEvent(StateEvent.delay);
                delayed = true;
            }
        }
    }
}
