using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public class SelectorNode : CompositeNode
    {
        public SelectorNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
        }

        public override void NodeTick()
        {
            base.NodeTick();
            for(int ii = 0; ii < children.Count; ii++)
            {
                BTNode child = children[ii];
                if(ii > activeChildIndex)
                {
                    break;
                }
                else if(child.enterTest == null)
                {
                    continue;
                }
                else if(!child.IsActive && child.enterTest.Invoke())
                {
                    ExitChildNode();
                    activeChildIndex = ii;
                    EnterChildNode();
                }
                else if(child.IsActive && !child.enterTest.Invoke())
                {
                    ChooseNextNode(Status.FAILED);
                }
            }
        }

        protected override int GetNextIndex(Status newStatus)
        {
            if(newStatus == Status.SUCCESS)
            {
                ChangeStatus(Status.SUCCESS);
                return -1;
            }

            int newIndex = activeChildIndex;
            while(newIndex < children.Count - 1)
            {
                newIndex++;
                if(children[newIndex].CanEnter())
                {
                    return newIndex;
                }
            }
            ChangeStatus(Status.FAILED);
            return -1;
        }
    }
}
