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

        #region //Node transitioning
        public TestNode NextNode()
        {
            int index = children.FindIndex(x => x.IsActive);
            TestNode nextChild;
            if(index == children.Count - 1)
            {
                nextChild = children[0];
            }
            else
            {
                nextChild = children[index + 1];
            }
            return nextChild.GetRandomChild();
        }

        private TestNode GetRandomChild()
        {
            if(children.Count == 0)
            {
                return this;
            }
            else
            {
                int randomChildNo = UnityEngine.Random.Range(0, children.Count);
                return children[randomChildNo].GetRandomChild();
            }
        }
        #endregion
    }
}