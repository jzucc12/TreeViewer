using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Rotates the AI to face its target
    /// </summary>
    public class RotateToNode : LeafNode
    {
        Transform target;
        float tolerance;
        public RotateToNode(string nodeName, BehaviorTree owner, Transform target, float tolerance) : base(nodeName, owner)
        {
            this.target = target;
            this.tolerance = tolerance;
        }

        public override void FixedTick()
        {
            base.FixedTick();
            if(owner.RotateTowards(target, tolerance))
            {
                ChangeStatus(NodeStatus.SUCCESS);
            }
        }
    }
}
