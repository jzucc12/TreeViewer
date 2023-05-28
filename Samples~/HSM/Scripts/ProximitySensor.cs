using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// HSM for sensors that detect player proximity. Tree can only be viewed in play mode
    /// </summary>
    public class ProximitySensor : HSM
    {
        [SerializeField] private float waitTime = 2f;


        private void Awake()
        {
            //Player far
            var playerFar = new BaseState(this, "Player far");
            var relaxed = new BaseState(this, "Relaxed");
            relaxed.AddDecorator(new AnimatorDecorator(this, "Relaxed"));
            var wait = new BaseState(this, "Wait");
            wait.AddDecorator(new DelayDecorator(this, waitTime));
            wait.AddDecorator(new AnimatorDecorator(this, "Wait"));

            playerFar.AddChild(wait);
            playerFar.AddChild(relaxed);
            playerFar.AddTransition(StateEvent.delay, wait, relaxed);

            //Player close
            var playerClose = new BaseState(this, "PlayerClose");
            var caution = new BaseState(this, "Caution");
            caution.AddDecorator(new DelayDecorator(this, waitTime));
            caution.AddDecorator(new AnimatorDecorator(this, "Caution"));
            var alert = new BaseState(this, "Alert");
            alert.AddDecorator(new AnimatorDecorator(this, "Alert"));

            playerClose.AddChild(caution);
            playerClose.AddChild(alert);
            playerClose.AddTransition(StateEvent.delay, caution, alert);

            //Root
            root = new BaseState(this, "Root");
            root.AddChild(playerFar);
            root.AddChild(playerClose);
            root.AddTransition(StateEvent.collideEnter, playerFar, playerClose);
            root.AddTransition(StateEvent.collideExit, playerClose, playerFar);
            root.EnterState(false);
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.GetComponent<CharacterStateMachine>() != null)
            {
                InvokeStateEvent(StateEvent.collideEnter);
            }
        }

        private void OnTriggerExit2D(Collider2D other) 
        {
            if(other.GetComponent<CharacterStateMachine>() != null)
            {
                InvokeStateEvent(StateEvent.collideExit);
            }
        }
    }
}
