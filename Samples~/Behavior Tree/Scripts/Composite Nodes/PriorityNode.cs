namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Always runs the first possible child node it can. Returns success if any child succeeds.
    /// </summary>
    public class PriorityNode : CompositeNode
    {
        public PriorityNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
        }

        public override void NodeTick()
        {
            base.NodeTick();
            //Check for first node that can be entered
            for(int ii = 0; ii < children.Count; ii++)
            {
                //Only check nodes prior to the active node
                if(ii > activeChildIndex)
                {
                    break;
                }

                //Leave the current node if its condition is broken
                else if(ii == activeChildIndex && !children[ii].CanEnter())
                {
                    ChooseNextNode(NodeStatus.FAILED);
                }

                //Enter this node if its condition is met
                else if(ii < activeChildIndex && children[ii].CanEnter())
                {
                    ExitChildNode();
                    activeChildIndex = ii;
                    EnterChildNode();
                }
            }
        }

        protected override int GetNextIndex(NodeStatus newStatus)
        {
            if(newStatus == NodeStatus.SUCCESS)
            {
                ChangeStatus(NodeStatus.SUCCESS);
                return -1;
            }

            //Find the next index that can be entered
            //Fails if it runs out of options
            int newIndex = activeChildIndex;
            while(newIndex < children.Count - 1)
            {
                newIndex++;
                if(children[newIndex].CanEnter())
                {
                    return newIndex;
                }
            }
            ChangeStatus(NodeStatus.FAILED);
            return -1;
        }
    }
}
