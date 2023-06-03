using System;
using System.Collections.Generic;
using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public abstract class BTNode : ITreeNodeViewer
    {
        public bool IsActive { get; private set; }
        public event Action<Status> OnStatusChange;
        public BTNode parent;
        protected BehaviorTree owner;
        private string nodeName;
        private Status myStatus;
        private bool activeChanged = false;
        public Func<bool> enterTest { get; private set; }


        public BTNode(string nodeName, BehaviorTree owner)
        {
            this.owner = owner;
            this.nodeName = nodeName;
        }

        public void EnterNode()
        {
            ChangeStatus(Status.RUNNING);
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

        public virtual void NodeTick()
        {

        }

        public virtual void FixedTick()
        {

        }

        public virtual void ResetNode()
        {

        }

        protected void ChangeStatus(Status newStatus)
        {
            myStatus = newStatus;
            OnStatusChange?.Invoke(newStatus);
        }

        public void ChangeEnterCondition(Func<bool> test)
        {
            enterTest = test;
        }

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

        public abstract IEnumerable<BTNode> GetChildren();

        private void MakeDirty()
        {
            activeChanged = true;
            parent?.MakeDirty();
        }

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

    public enum Status
    {
        SUCCESS,
        RUNNING,
        FAILED
    }
}
