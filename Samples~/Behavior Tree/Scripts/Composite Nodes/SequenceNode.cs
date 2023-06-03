using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
        }

        protected override int GetNextIndex(Status newStatus)
        {
            if(newStatus == Status.FAILED)
            {
                ChangeStatus(Status.FAILED);
                return -1;
            }

            int newIndex = activeChildIndex + 1;
            if(newIndex == children.Count)
            {
                ChangeStatus(Status.SUCCESS);
                return -1;
            }
            else if(children[newIndex].CanEnter())
            {
                return newIndex;
            }
            else
            {
                ChangeStatus(Status.FAILED);
                return -1;
            }
        }
    }
}
