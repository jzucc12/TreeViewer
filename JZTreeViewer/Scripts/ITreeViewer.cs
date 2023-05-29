using System.Collections.Generic;

namespace JZ.TreeViewer
{
    /// <summary>
    /// Implemented by the tree itself
    /// If applied to a monobehaviour, clicking on that object in the hierarchy will show its nodes
    /// </summary>
    public interface ITreeViewer
    {
        /// <returns>The tree's name</returns>
        string GetTreeName();

        /// <returns>All nodes within the tree. First node returned must be the root node</returns>
        IEnumerable<ITreeNodeViewer> GetAllNodes();
    }
}