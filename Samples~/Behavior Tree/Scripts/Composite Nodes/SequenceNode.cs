namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Completes each node in order. Returns failure if any child fails or can't be entered
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        public SequenceNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
        }

        protected override int GetNextIndex(NodeStatus newStatus)
        {
            if(newStatus == NodeStatus.FAILED)
            {
                ChangeStatus(NodeStatus.FAILED);
                return -1;
            }

            //Increment index and succeed if you've run out of children
            //but fail if the next child can't be entered
            int newIndex = activeChildIndex + 1;
            if(newIndex == children.Count)
            {
                ChangeStatus(NodeStatus.SUCCESS);
                return -1;
            }
            else if(children[newIndex].CanEnter())
            {
                return newIndex;
            }
            else
            {
                ChangeStatus(NodeStatus.FAILED);
                return -1;
            }
        }
    }
}
