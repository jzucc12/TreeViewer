using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base state for UI State system
    /// </summary>
    public class UIBaseState : ITreeNodeViewer
    {
        public bool IsActive { get; protected set; }
        private string nodeName;
        protected bool changed;
        private UIBaseState parent;
        public List<UIBaseState> children { get; protected set; } = new List<UIBaseState>();


        public UIBaseState(string nodeName)
        {
            this.nodeName = nodeName;
        }

        #region //Children states
        public void AddChild(UIBaseState child)
        {
            child.parent = this;
            children.Add(child);
        }

        /// <returns>Depth first list of children under this one in the entire tree</returns>
        public List<UIBaseState> GetAllChildren()
        {
            //Add your children to the list
            List<UIBaseState> allChildren = new List<UIBaseState>();
            allChildren.AddRange(children);

            //Add all children under this in the entire tree
            foreach(UIBaseState child in children)
            {
                allChildren.AddRange(child.GetAllChildren());
            }
            return allChildren;
        }
        #endregion

        #region //State transitioning
        public void EnterState()
        {
            IsActive = true;
            SetDirty();
            OnEnter();
        }

        public void ExitState()
        {
            IsActive = false;
            SetDirty();
            OnExit();
        }

        private void SetDirty()
        {
            changed = true;
            parent?.SetDirty();
        }

        /// <summary>
        /// Extra effects when entered. Enters all child states by default.
        /// </summary>
        protected virtual void OnEnter()
        {
            foreach(UIBaseState child in children)
            {
                child.EnterState();
            }
        }

        /// <summary>
        /// Extra effects when exiting. Exits all child states by default.
        /// </summary>
        protected virtual void OnExit()
        {
            foreach(UIBaseState child in children)
            {
                child.ExitState();
            }
        }
        #endregion

        #region //Interface specific
        public string GetNodeName()
        {
            return nodeName;
        }

        public bool IsNodeDirty()
        {
            bool didChange = changed;
            changed = false;
            return didChange;
        }

        public bool IsChildOf(ITreeNodeViewer node)
        {
            return parent == node;
        }
        #endregion
    }
}
