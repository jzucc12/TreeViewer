using System.Collections.Generic;

namespace JZ.TreeViewer
{
    /// <summary>
    /// Example node class for the GUI
    /// </summary>
    public class TestNode : ITreeNodeViewer
    {
        private string nodeName;
        public bool IsActive { get; private set; }
        private List<TestNode> children = new List<TestNode>();
        private TestNode parent;
        private bool isDirty;


        #region //Node specific
        public TestNode(string nodeName)
        {
            this.nodeName = nodeName;
            IsActive = false;
        }

        public void AddChild(TestNode child)
        {
            children.Add(child);
            child.parent = this;
        }

        public void ActivateNode()
        {
            IsActive = true;
            MakeDirty();
            parent?.ActivateNode();
        }

        public void DeactivateNode()
        {
            IsActive = false;
            MakeDirty();
            parent?.DeactivateNode();
        }

        private void MakeDirty()
        {
            isDirty = true;
        }

        private void CleanDirty()
        {
            isDirty = false;
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
            bool dirty = isDirty;
            CleanDirty();
            return dirty;
        }
        #endregion
    }
}