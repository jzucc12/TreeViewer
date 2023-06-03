namespace JZ.TreeViewer.Samples
{
    public class RepeaterNode : DecoratorNode
    {
        public RepeaterNode(string nodeName, BehaviorTree owner, BTNode child) : base(nodeName, owner, child)
        {
        }

        protected override void OnChildStatusChange(Status childStatus)
        {
            if(childStatus != Status.RUNNING)
            {
                child.OnExitNode();
                child.OnEnterNode();
            }
        }
    }
}