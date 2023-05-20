namespace JZ.TreeViewer
{
    /// <summary>
    /// Implemented by the nodes of the tree
    /// </summary>
    public interface ITreeNodeViewer
    {
        /// <returns>True if this node is currently active</returns>
        bool IsActive { get; }

        /// <returns>Name of the node</returns>
        string GetNodeName(); 

        /// <returns>True if the node or any of its children have changed in anyway</returns>
        bool IsNodeDirty();

        /// <param name="node">potential parent node</param>
        /// <returns>If this node is a child of the parameter node</returns>
        bool IsChildOf(ITreeNodeViewer node);
    }
}