using System;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Changes input shape
    /// </summary>
    public class ShapeChanger : MonoBehaviour
    {
        [SerializeField] private Shape startShape;
        [SerializeField] private Image[] images;
        public Shape currentShape { get; private set; } = Shape.Circle;
        public event Action OnShapeChange;


        private void OnValidate()
        {
            SetShape(startShape);
        }

        public void SetNextShape()
        {
            int newIndex = (int)currentShape + 1;
            if(newIndex >= Enum.GetValues(typeof(Shape)).Length)
            {
                newIndex = 0;
            }
            SetShape((Shape)newIndex);
        }

        public void SetShape(Shape newShape)
        {
            images[(int)currentShape].enabled = false;
            images[(int)newShape].enabled = true;
            currentShape = newShape;
            OnShapeChange?.Invoke();
        }
    }
}