using System;
using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
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

        public override void OnEnterNode()
        {
            activeChildIndex = -1;
            ChooseNextNode(Status.RUNNING);
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

        public void AddChild(BTNode child)
        {
            children.Add(child);
            child.parent = this;
        }

        private void OnChildStatusChange(Status childStatus)
        {
            if (childStatus == Status.RUNNING)
            {
                return;
            }
            ChooseNextNode(childStatus);
        }

        protected void ChooseNextNode(Status childStatus)
        {
            ExitChildNode();
            int newIndex = GetNextIndex(childStatus);
            if(newIndex != -1)
            {
                activeChildIndex = newIndex;
                EnterChildNode();
            }
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

        protected abstract int GetNextIndex(Status newStatus);

        public override IEnumerable<BTNode> GetChildren()
        {
            return children;
        }
    }
}
