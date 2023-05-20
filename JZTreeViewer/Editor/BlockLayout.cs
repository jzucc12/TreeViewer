using System;

#if UNITY_EDITOR
namespace JZ.TreeViewer.Editor
{
    /// <summary>
    /// Holds data on block positioning for saving layouts
    /// </summary>
    [Serializable]
    public struct BlockLayout
    {
        public bool showingChildren;
        public float xPos;
        public float yPos;
        public float zPos;

        public BlockLayout(TNodeBlock block, bool showingChildren)
        {
            xPos = block.transform.position.x;
            yPos = block.transform.position.y;
            zPos = block.transform.position.z;
            this.showingChildren = showingChildren;
        }
    }
}
#endif