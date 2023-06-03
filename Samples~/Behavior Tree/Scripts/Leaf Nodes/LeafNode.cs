using System.Collections.Generic;
using UnityEngine;

namespace JZ.TreeViewer.Samples
{
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
