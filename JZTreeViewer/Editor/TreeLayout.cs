using System;
using System.Collections.Generic;
using JZ.Common;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace JZ.TreeViewer.Editor
{
    /// <summary>
    /// Holds data on a tree for saving layouts
    /// </summary>
    [Serializable]
    public struct TreeLayout
    {
        public List<BlockLayout> blockLayouts;
        public float zoom;
        public StyleTransformOrigin transformOrigin;
        public float panPosX;
        public float panPosY;
        public float panPosZ;

        public TreeLayout(PanAndZoomView container)
        {
            zoom = container.transform.scale.x;
            transformOrigin = container.style.transformOrigin;
            panPosX = container.transform.position.x;
            panPosY = container.transform.position.y;
            panPosZ = container.transform.position.z;
            blockLayouts = new List<BlockLayout>();
            foreach(TNodeBlock child in container.GetElements())
            {
                blockLayouts.Add(child.CreateLayout());
            }
        }

        public void LoadLayout(PanAndZoomView container, List<TNodeBlock> blocks)
        {
            //Positions the block container
            container.transform.scale = Vector3.one * zoom;
            container.style.transformOrigin = transformOrigin;
            container.transform.position = new Vector3(panPosX, panPosY, panPosZ);
            container.transform.position = new Vector3(panPosX, panPosY, panPosZ);

            //Sets up blocks (if block order changes, the reset will be messed up)
            for(int ii = 0; ii < blockLayouts.Count; ii++)
            {
                if(ii > blocks.Count - 1) break;
                blocks[ii].LoadFromLayout(blockLayouts[ii]);
            }
        }
    }
}
#endif