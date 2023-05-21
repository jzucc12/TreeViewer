using UnityEngine.UIElements;
using JZ.Common.Editor;
using JZ.Common;
using System.Collections.Generic;
using UnityEngine;

namespace JZ.TreeViewer.Editor
{
    /// <summary>
    /// Visual block on of the node on the GUI
    /// </summary>
    public class TNodeBlock : VisualElement
    {
        private VisualTreeAsset sourceTree;
        private VisualElement actualBlock; //First child under this and the source of the uxml tree
        private ITreeNodeViewer myNode;
        private TreeSettingManager settingManager;
        private DragAndDropManipulator dragAndDrop;

        private TNodeBlock parentBlock;
        private List<TNodeBlock> childBlocks = new List<TNodeBlock>();
        private Button childButton;
        private bool showingChildren = true;

        #region //Set up
        public TNodeBlock(ITreeNodeViewer myNode, TreeSettingManager settingManager)
        {
            //Create tree and set up styles
            this.SetUxmlAndUss("Tree Viewer uxml and uss", "NodeBlock.uxml", "Tree Viewer Tool Style.uss");
            this.style.alignItems = Align.Center;
            this.style.position = Position.Absolute;
            actualBlock = this[0]; //Gets the first child
            generateVisualContent += OnGenerateVisualContent;

            //Set up context menu
            this.AddManipulator(new ContextualMenuManipulator(GenerateContextMenu));

            //Set up node parameters
            this.myNode = myNode;
            Label nodeName = this.Q<Label>("node-name");
            nodeName.text = myNode.GetNodeName();

            //Set up show/hide button
            childButton = this.Q<Button>("show-child");
            childButton.style.display = DisplayStyle.None;

            //Settings
            this.settingManager = settingManager;
            settingManager.AddSettingEvent(TreeSetting.inactiveColor, UpdateBlock);
            settingManager.AddSettingEvent(TreeSetting.activeColor, UpdateBlock);
            settingManager.AddSettingEvent(TreeSetting.lineThickness, UpdateBlock);
        }

        private void GenerateContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Reset Child Positions", ResetChildren);
        }

        public void AddDragAndDrop()
        {
            dragAndDrop = new DragAndDropManipulator(this);
            dragAndDrop.OnDrag += RepaintParent;
            this.AddManipulator(dragAndDrop);
        }
        #endregion

        #region //Child block modification
        public void AddChildBlock(TNodeBlock child)
        {
            childBlocks.Add(child);
            dragAndDrop?.AddChild(child.dragAndDrop);
            child.parentBlock = this;

            if(childBlocks.Count == 1)
            {
                childButton.style.display = DisplayStyle.Flex;
                childButton.clicked += ToggleShowChildren;
            }
        }

        private void ResetChildren(DropdownMenuAction action)
        {
            ResetChildren();
            UpdateBlock();
        }

        /// <summary>
        /// Places children in default positions based on the x-gap and y-gap settings
        /// </summary>
        public void ResetChildren()
        {
            float noXGapIndex = (childBlocks.Count / 2f) - 0.5f; //Child index of block directly below the parent, if one exists
            int depth = DistanceFromRoot(); //Used to narrow x-gap the further away from the root this block is

            for(int ii = 0; ii < childBlocks.Count; ii++)
            {
                float xGapMultiplier = (ii - noXGapIndex); // General placement based on which child this is
                xGapMultiplier /= (1 + (depth * childBlocks.Count * 0.75f)); //Adjusting for depth. Empirically determined

                childBlocks[ii].transform.position = new Vector2(transform.position.x + settingManager.GetXGap() * xGapMultiplier,
                                                                 transform.position.y + settingManager.GetYGap());
                childBlocks[ii].ResetChildren();
            }
            
            //Ensures the block was not moved outside of the view
            dragAndDrop?.BindToParent();
        }

        /// <returns>Distance from the root block of the tree</returns>
        private int DistanceFromRoot()
        {
            int count = 0;
            TNodeBlock parent = parentBlock;
            while(parent != null)
            {
                parent = parent.parentBlock;
                count++;
            }
            
            return count;
        }
        #endregion

        #region //Showing child blocks
        private void ToggleShowChildren()
        {
            showingChildren = !showingChildren;
            UpdateBlock();
        }

        public void SetShowButton(bool newNode)
        {
            showingChildren = newNode;
        }

        private void ShowChildren()
        {
            //Override button depending on settings
            if(settingManager.GetShowActiveNode() && myNode.IsActive)
            {
                showingChildren = true;
            }

            bool show = visible && showingChildren;
            childButton.text = show ? "Hide" : "Show";
            foreach(TNodeBlock child in childBlocks)
            {
                child.visible = show;
            }
        }
        #endregion

        #region //Updating block display
        /// <summary>
        /// Updates the color and splines of this and all child blocks under it
        /// </summary>
        public void UpdateBlock()
        {
            //Set border colo
            Color lineColor = settingManager.GetActiveColor(myNode.IsActive);
            actualBlock.style.borderBottomColor = lineColor;
            actualBlock.style.borderTopColor = lineColor;
            actualBlock.style.borderLeftColor = lineColor;
            actualBlock.style.borderRightColor = lineColor;

            ShowChildren();
            MarkDirtyRepaint();
            foreach(TNodeBlock child in childBlocks)
            {
                child.UpdateBlock();
            }
        }

        // Only called when this block is dragged so the parent can fix its spline
        private void RepaintParent()
        {
            parentBlock?.MarkDirtyRepaint();
        }

        /// <summary>
        /// Creates splines travelling to child blocks
        /// </summary>
        /// <param name="mgc"></param>
        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            //This check needs to be in this method, nowhere else.
            //Reason being that immediately returning deletes all splines.
            //Putting this check elsewhere would leave splines comint out of this block and connecting to nothing
            if(!showingChildren)
            {
                return;
            }

            int resolution = 50;
            foreach(TNodeBlock child in childBlocks)
            {
                //Make control point offset
                Vector3 direction = child.transform.position - transform.position;
                Vector3 controlOffset = direction;
                controlOffset.x = 0;
                controlOffset.y *= 0.7f;

                //Make control points
                Vector3 p1 = contentRect.center;
                Vector3 p4 = p1 + direction;
                Vector3 p2 = p1 + controlOffset;
                Vector3 p3 = p4 - controlOffset;

                //Determine spline points
                Vector3[] points = new Vector3[resolution];
                for(int ii = 0; ii < resolution; ii++)
                {
                    float t = ii / (resolution - 1f);
                    points[ii] = JZMath.CubicInterp(p1, p2, p3, p4, t);
                    points[ii].z = Vertex.nearZ;
                }

                //Draw bezier
                Color lineColor = settingManager.GetActiveColor(child.myNode.IsActive);
                mgc.MakeVEMesh(points, resolution, settingManager.GetLineThickness(), lineColor);
            }
        }
        #endregion
    
        #region //On node change
        // Used to alert the gui a node has changed and that it should update
        /// <returns>If the node or its children have been changed</returns>
        public bool IsNodeDirty()
        {
            return myNode.IsNodeDirty();
        }

        // Used by the node history log to record the new active node
        /// <returns>The deepest active node in relation to this one</returns>
        public string FindDeepestActive()
        {
            if(!myNode.IsActive) return "I'm not active";

            foreach(TNodeBlock child in childBlocks)
            {
                if(child.myNode.IsActive)
                {
                    return child.FindDeepestActive();
                }
            }

            return myNode.GetNodeName();
        }
        #endregion
    
        #region //Saving and loading layouts
        public BlockLayout CreateLayout()
        {
            return new BlockLayout(this, showingChildren);
        }

        public void LoadFromLayout(BlockLayout layout)
        {
            transform.position = new Vector3(layout.xPos, layout.yPos, layout.zPos);
            showingChildren = layout.showingChildren;
            MarkDirtyRepaint();
        }
        #endregion
    }
}