using System.Collections;
using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base state for the Shape HSM
    /// </summary>
    public class ShapeBaseState : ITreeNodeViewer, IEnumerable<ITreeNodeViewer>
    {
        public bool IsActive { get; private set; }
        private string stateName;
        private ShapeBaseState parent;
        private List<ShapeBaseState> children = new List<ShapeBaseState>();
        private bool isDirty = false;


        #region //State specific
        public ShapeBaseState(string stateName, ShapeBaseState parent = null)
        {
            this.stateName = stateName;
            parent?.AddChild(this);
        }

        private void AddChild(ShapeBaseState state)
        {
            state.parent = this;
            children.Add(state);
        }

        public void EnterState()
        {
            IsActive = true;
            isDirty = true;
            parent?.EnterState();
        }

        public void ExitState()
        {
            IsActive = false;
            isDirty = true;
            parent?.ExitState();
        }

        public IEnumerator<ITreeNodeViewer> GetEnumerator()
        {
            yield return this;
            for (int ii = 0; ii < children.Count; ii++)
            {
                if (children[ii] != null)
                {
                    foreach(ITreeNodeViewer state in children[ii])
                    {
                        yield return state;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region //Interface specific
        public string GetNodeName()
        {
            return stateName;
        }

        public bool IsChildOf(ITreeNodeViewer state)
        {
            return parent == state;
        }

        public bool IsNodeDirty()
        {
            bool dirty = isDirty;
            isDirty = false;
            return dirty;
        }
        #endregion
    }
}