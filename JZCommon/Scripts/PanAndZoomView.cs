using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace JZ.Common
{
    /// <summary>
    /// Visual element that allows you pan and zoom elements within its element container.
    /// </summary>
    public class PanAndZoomView : VisualElement
    {
        private VisualElement elementContainer;
        private VisualElement boundingElement; 
        private float currentZoom = 1;

        //Factory variables
        public float viewWidth { get; set; }
        public float viewHeight { get; set; }
        public float minZoom {get; set; }
        public float maxZoom {get; set; }
        public float zoomSpeed { get; set; }
        public MouseButtonValues panButton { get; set; }


        #region //Construction
        public new class UxmlFactory : UxmlFactory<PanAndZoomView, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatAttributeDescription m_Width =
                new UxmlFloatAttributeDescription { name = "view-width", defaultValue = 6000 };
            UxmlFloatAttributeDescription m_Height =
                new UxmlFloatAttributeDescription { name = "view-height", defaultValue = 6000 };
            UxmlFloatAttributeDescription m_MinZoom =
                new UxmlFloatAttributeDescription { name = "min-zoom", defaultValue = 0.2f };
            UxmlFloatAttributeDescription m_MaxZoom =
                new UxmlFloatAttributeDescription { name = "max-zoom", defaultValue = 3f };
            UxmlFloatAttributeDescription m_ZoomSpeed =
                new UxmlFloatAttributeDescription { name = "zoom-speed", defaultValue = 0.05f };
            UxmlEnumAttributeDescription<MouseButtonValues> m_PanButton =
                new UxmlEnumAttributeDescription<MouseButtonValues> { name = "pan-button", defaultValue = MouseButtonValues.Middle };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as PanAndZoomView;

                ate.viewWidth = m_Width.GetValueFromBag(bag, cc);
                ate.viewHeight = m_Height.GetValueFromBag(bag, cc);
                ate.style.minHeight = ate.viewHeight;
                ate.style.minWidth = ate.viewWidth;

                ate.minZoom = m_MinZoom.GetValueFromBag(bag, cc);
                ate.maxZoom = m_MaxZoom.GetValueFromBag(bag, cc);
                ate.zoomSpeed = m_ZoomSpeed.GetValueFromBag(bag, cc);
                ate.panButton = m_PanButton.GetValueFromBag(bag, cc);
            }
        }

        public PanAndZoomView()
        {
            elementContainer = new VisualElement();
            elementContainer.name = "Element container";
            elementContainer.style.alignItems = Align.Center;
            elementContainer.style.width = Length.Percent(100);
            elementContainer.style.height = Length.Percent(100);
            this.Add(elementContainer);

            RegisterCallback<WheelEvent>(OnZoom);
            RegisterCallback<MouseMoveEvent>(OnPan);
        }
        #endregion

        #region //Pan and zoom
        private void OnPan(MouseMoveEvent evt)
        {
            if(evt.pressedButtons == (int)panButton)
            {
                elementContainer.transform.position += (Vector3)evt.mouseDelta;
            }
        }

        private void OnZoom(WheelEvent evt)
        {
            float newZoom = currentZoom - evt.delta.y * zoomSpeed; 

            //Modified version from Running |V|an on Unity forums
            //Repositions window according to the mouse position
            var newOrigin = this.ChangeCoordinatesTo(this, evt.localMousePosition);
            var oldBound = elementContainer.worldBound;
            elementContainer.style.transformOrigin = new TransformOrigin(newOrigin.x, newOrigin.y, 0);
            var deltaPosition = oldBound.position - elementContainer.worldBound.position;
            elementContainer.transform.position += (Vector3)deltaPosition;
        
            //Sets new zoom
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            currentZoom = newZoom;
            elementContainer.transform.scale = Vector3.one * newZoom;
        }

        public void ResetView()
        {
            currentZoom = 1;
            elementContainer.transform.position = Vector3.zero;
            elementContainer.transform.scale = Vector3.one;
        }
        #endregion

        #region //Child element modification
        public void AddElement(VisualElement ve)
        {
            elementContainer.Add(ve);
        }

        public void RemoveElement(VisualElement ve)
        {
            elementContainer.Remove(ve);
        }

        public void RemoveElementAt(int index)
        {
            elementContainer.RemoveAt(index);
        }

        public void InsertElement(int index, VisualElement ve)
        {
            elementContainer.Insert(index, ve);
        }

        public void ClearElements()
        {
            elementContainer.Clear();
        }

        public IEnumerable<VisualElement> GetElements()
        {
            return elementContainer.Children();
        }
        #endregion
    }
}