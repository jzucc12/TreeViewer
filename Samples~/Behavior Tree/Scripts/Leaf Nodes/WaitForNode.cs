using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Delays further actions for a given amount of time
    /// </summary>
    public class WaitForNode : LeafNode
    {
        private float waitTime;
        private float currentTime;
        public WaitForNode(string nodeName, BehaviorTree owner, float time) : base(nodeName, owner)
        {
            waitTime = time;
        }

        public override void OnEnterNode()
        {
            base.OnEnterNode();
            currentTime = 0;
        }

        public override void FixedTick()
        {
            base.FixedTick();
            currentTime += Time.deltaTime;
            if(currentTime >= waitTime)
            {
                ChangeStatus(NodeStatus.SUCCESS);
            }
        }
    }
}
