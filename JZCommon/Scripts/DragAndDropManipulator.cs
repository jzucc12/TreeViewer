using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace JZ.Common
{
    /// <summary>
    /// Visual element manipulator that allows you to drag the element
    /// </summary>
    public class DragAndDropManipulator : PointerManipulator
    {
        private List<DragAndDropManipulator> children = new List<DragAndDropManipulator>();

        private event Action<Vector3> OnDown;
        private event Action<Vector3> OnMove;
        public event Action OnDrag;

        private bool dragging;
        private bool bindToParent;
        private MouseButtonValues dragButton;
        private Vector2 targetDragStart;
        private Vector3 mouseDragStart;


        #region //Set up
        public DragAndDropManipulator(VisualElement target, bool boundToParent = false, MouseButtonValues dragButton = MouseButtonValues.LeftClick)
        {
            this.target = target;
            this.bindToParent = boundToParent;
            this.dragButton = dragButton;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }
        #endregion

        #region //Drag and drop
        /// <summary>
        /// Captures target and alerts all child manipulators to the capture
        /// </summary>
        /// <param name="evt"></param>
        private void PointerDownHandler(PointerDownEvent evt)
        {
            if(evt.pressedButtons == (int)dragButton)
            {
                targetDragStart = target.transform.position;
                mouseDragStart = evt.position;
                target.CapturePointer(evt.pointerId);
                OnDown?.Invoke(evt.position);
                dragging = true;
            }
        }

        /// <summary>
        /// Drags the target element
        /// </summary>
        /// <param name="evt"></param>
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
            {
                MoveElement(evt.position);
            }
            OnDrag?.Invoke();
        }

        /// <summary>
        /// Moves target element and all child manipulators
        /// </summary>
        /// <param name="mousePos"></param>
        private void MoveElement(Vector3 mousePos)
        {
            Vector3 pointerDelta = mousePos - mouseDragStart;
            target.transform.position = new Vector2(
                targetDragStart.x + pointerDelta.x / target.parent.transform.scale.x,
                targetDragStart.y + pointerDelta.y / target.parent.transform.scale.y);

            BindToParent();
            OnMove?.Invoke(mousePos);
        }

        /// <summary>
        /// Keeps the target element within its parent's bounds
        /// </summary>
        public void BindToParent()
        {
            if (bindToParent)
            {
                //Bounding has to take local element position and parent border width into account
                target.transform.position = new Vector2(
                    Mathf.Clamp(target.transform.position.x, -target.layout.xMin + target.parent.resolvedStyle.borderLeftWidth,
                                target.parent.layout.width - target.layout.xMin - target.layout.width - target.parent.resolvedStyle.borderRightWidth),
                    Mathf.Clamp(target.transform.position.y, -target.layout.yMin + target.parent.resolvedStyle.borderTopWidth,
                                target.parent.layout.height - target.layout.yMin - target.layout.height - target.parent.resolvedStyle.borderBottomWidth));
            }
        }

        /// <summary>
        /// Releases the target element
        /// </summary>
        /// <param name="evt"></param>
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (dragging && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
            }
        }
        #endregion

        #region //Child management
        /// <summary>
        /// Adds manipulator to be dragged with this one
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(DragAndDropManipulator child)
        {
            children.Add(child);
            OnDown += child.RemoteDown;
            OnMove += child.MoveElement;
        }

        /// <summary>
        /// Alerts children manipulators that this element can be dragged
        /// </summary>
        /// <param name="mousePos"></param>
        private void RemoteDown(Vector3 mousePos)
        {
            targetDragStart = target.transform.position;
            mouseDragStart = mousePos;
            OnDown?.Invoke(mousePos);
        }
        #endregion
    }
}