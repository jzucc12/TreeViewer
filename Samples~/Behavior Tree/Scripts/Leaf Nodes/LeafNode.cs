using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Base node that usually interacts with the AI.
    /// </summary>
    public abstract class LeafNode : BTNode
    {
        protected LeafNode(string nodeName, BehaviorTree owner) : base(nodeName, owner)
        {
        }

        public override IEnumerable<BTNode> GetChildren()
        {
            return new List<BTNode>();
        }

        public override void OnEnterNode()
        {
        }

        public override void OnExitNode()
        {
        }
    }
}
