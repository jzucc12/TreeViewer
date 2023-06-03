namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Node that puts its child on an infinite loop
    /// </summary>
    public class RepeaterNode : DecoratorNode
    {
        public RepeaterNode(string nodeName, BehaviorTree owner, BTNode child) : base(nodeName, owner, child)
        {
        }

        protected override void OnChildStatusChange(NodeStatus childStatus)
        {
            if(childStatus != NodeStatus.RUNNING)
            {
                child.OnExitNode();
                child.OnEnterNode();
            }
        }
    }
}