using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Node that has multiple chldren
    /// </summary>
    public abstract class CompositeNode : BTNode
    {
        protected List<BTNode> children = new List<BTNode>();
        protected int activeChildIndex;
        private BTNode activeChild { get {
            if(activeChildIndex >= 0 && activeChildIndex <= children.Count - 1)
            {
                return children[activeChildIndex];
            }
            else
            {
                return null;
            }
        }}

        protected CompositeNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
            activeChildIndex = 0;
        }

        #region //From base node
        public override void OnEnterNode()
        {
            activeChildIndex = -1;
            ChooseNextNode(NodeStatus.RUNNING);
        }

        public override void OnExitNode()
        {
            ExitChildNode();
        }

        public override void NodeTick()
        {
            base.NodeTick();
            activeChild?.NodeTick();
        }

        public override void FixedTick()
        {
            base.FixedTick();
            activeChild?.FixedTick();
        }

        public override IEnumerable<BTNode> GetChildren()
        {
            return children;
        }
        #endregion

        #region //Child nodes
        public void AddChild(BTNode child)
        {
            children.Add(child);
            child.parent = this;
        }

        protected void ExitChildNode()
        {
            if(activeChild != null)
            {
                activeChild.OnStatusChange -= OnChildStatusChange;
                activeChild.ExitNode();
            }
        }

        protected void EnterChildNode()
        {
            if(activeChild != null)
            {
                activeChild.OnStatusChange += OnChildStatusChange;
                activeChild.EnterNode();
            }
        }
        #endregion

        #region //Choosing future child nodes to enter
        private void OnChildStatusChange(NodeStatus childStatus)
        {
            if (childStatus != NodeStatus.RUNNING)
            {
                ChooseNextNode(childStatus);
            }
        }

        protected void ChooseNextNode(NodeStatus childStatus)
        {
            ExitChildNode();
            int newIndex = GetNextIndex(childStatus);
            if(newIndex != -1)
            {
                activeChildIndex = newIndex;
                EnterChildNode();
            }
        }

        /// <returns>Next valid child node to enter</returns>
        protected abstract int GetNextIndex(NodeStatus newStatus);
        #endregion
    }
}
