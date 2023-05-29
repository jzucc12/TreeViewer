using System.Collections.Generic;
using UnityEngine.UIElements;

namespace JZ.TreeViewer
{
    /// <summary>
    /// Example tree class for the GUI.
    /// </summary>
    public class TestTree : ITreeViewer
    {
        private TestNode rootNode;
        private TestNode activeNode;
        private List<TestNode> nodes = new List<TestNode>();
        private string treeName;
    

        #region //Tree specific
        public TestTree(string smName, VisualElement view)
        {
            //Set up example Tree
            this.treeName = smName;

            rootNode = CreateNode("Test");
            var test2 = CreateNode("Test", rootNode);
            var test3 = CreateNode("Test", rootNode);
            var test4 = CreateNode("Test", rootNode);
            var test5 = CreateNode("Test", test2);
            var test6 = CreateNode("Test", test2);
            var test7 = CreateNode("Test", test6);
            var test8 = CreateNode("Test", test3);
            var test9 = CreateNode("Test", test8);
            var test10 = CreateNode("Test", test8);
            var test11 = CreateNode("Test", test3);
            var test12 = CreateNode("Test", test4);
            var test13 = CreateNode("Test", test4);
            var test14 = CreateNode("Test", test13);
            var test15 = CreateNode("Test", test14);
            ChangeNode(test10);

            //For node transitioning
            view.RegisterCallback<MouseDownEvent>(OnClick);
        }

        private TestNode CreateNode(string nodeName, TestNode parent = null)
        {
            TestNode newNode = new TestNode(nodeName);
            if(parent != null)
            {
                parent.AddChild(newNode);
            }
            nodes.Add(newNode);
            return newNode;
        }

        public void ChangeNode(TestNode newNode)
        {
            activeNode?.DeactivateNode();
            activeNode = newNode;
            activeNode.ActivateNode();
        }
        #endregion

        #region //Interface specific
        public string GetTreeName()
        {
            return treeName;
        }

        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            foreach(TestNode node in nodes)
            {
                yield return node;
            }
        }
        #endregion

        #region //Node transitioning
        private void OnClick(MouseDownEvent evt)
        {
            NextNode();
        }

        private void NextNode()
        {
            ChangeNode(rootNode.NextNode());
        }
        #endregion
    }
}