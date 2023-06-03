using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public class MoveToNode : LeafNode
    {
        Transform target;
        float tolerance;
        public MoveToNode(string nodeName, BehaviorTree owner, Transform target, float tolerance) : base(nodeName, owner)
        {
            this.target = target;
            this.tolerance = tolerance;
        }


        public override void FixedTick()
        {
            base.FixedTick();
            if(owner.MoveTowards(target, tolerance))
            {
                ChangeStatus(Status.SUCCESS);
            }
        }
    }
}
