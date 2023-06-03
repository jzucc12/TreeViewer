using System;
using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base node for the behavior tree
    /// </summary>
    public abstract class BTNode : ITreeNodeViewer
    {
        public bool IsActive { get; private set; }
        public event Action<NodeStatus> OnStatusChange;
        public BTNode parent;
        protected BehaviorTree owner;
        private string nodeName;
        private NodeStatus myStatus;
        private bool activeChanged = false;
        private Func<bool> enterTest;


        public BTNode(string nodeName, BehaviorTree owner)
        {
            this.owner = owner;
            this.nodeName = nodeName;
        }

        public virtual void NodeTick() { }
        public virtual void FixedTick() { } 

        #region //Entering and exiting
        public void EnterNode()
        {
            ChangeStatus(NodeStatus.RUNNING);
            MakeDirty();
            IsActive = true;
            OnEnterNode();
        }

        public void ExitNode()
        {
            MakeDirty();
            IsActive = false;
            OnExitNode();
        }
        public abstract void OnEnterNode();
        public abstract void OnExitNode();

        /// <summary>
        /// Condition for this node to be entered.
        /// </summary>
        /// <param name="test"></param>
        public void ChangeEnterCondition(Func<bool> test)
        {
            enterTest = test;
        }

        /// <returns>If this node can be entered. Returns true if no enter condition was set</returns>
        public bool CanEnter()
        {
            if(enterTest == null)
            {
                return true;
            }
            else
            {
                return enterTest.Invoke();
            }
        }
        #endregion

        #region //Node state
        protected void ChangeStatus(NodeStatus newStatus)
        {
            myStatus = newStatus;
            OnStatusChange?.Invoke(newStatus);
        }

        public abstract IEnumerable<BTNode> GetChildren();

        private void MakeDirty()
        {
            activeChanged = true;
            parent?.MakeDirty();
        }
        #endregion

        #region //Interface specific
        public string GetNodeName()
        {
            return nodeName;
        }

        public bool IsChildOf(ITreeNodeViewer node)
        {
            return parent == node;
        }

        public bool IsNodeDirty()
        {
            bool didChange = activeChanged;
            activeChanged = false;
            return didChange;
        }
        #endregion
    }
}
