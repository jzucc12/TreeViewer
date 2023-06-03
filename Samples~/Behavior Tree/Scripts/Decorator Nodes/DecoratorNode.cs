using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Node that has a single child that it modifies.
    /// </summary>
    public abstract class DecoratorNode : BTNode
    {
        protected BTNode child;


        protected DecoratorNode(string nodeName, BehaviorTree owner, BTNode child) : base(nodeName, owner)
        {
            this.child = child;
            child.parent = this;
            child.OnStatusChange += OnChildStatusChange;
        }

        public override void OnEnterNode()
        {
            child.EnterNode();
        }

        public override void OnExitNode()
        {
            child.ExitNode();
        }

        public override void NodeTick()
        {
            base.NodeTick();
            child.NodeTick();
        }

        public override void FixedTick()
        {
            base.FixedTick();
            child.FixedTick();
        }

        protected abstract void OnChildStatusChange(NodeStatus childStatus);

        public override IEnumerable<BTNode> GetChildren()
        {
            yield return child;
        }
    }
}
