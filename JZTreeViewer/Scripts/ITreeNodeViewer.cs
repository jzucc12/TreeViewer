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

        /// <returns>If thise node has changed in anyway. Change should propogate up to the root node/returns>
        bool IsNodeDirty();

        /// <param name="node">potential parent node</param>
        /// <returns>If this node is a child of the parameter node</returns>
        bool IsChildOf(ITreeNodeViewer node);
    }
}