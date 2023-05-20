using System.Collections.Generic;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// State used to pick which output image to display
    /// </summary>
    public class ShapeShowState : ShapeBaseState
    {
        private HashSet<Shape> myShapes = new HashSet<Shape>();
        public ShapeShowState(string stateName, ShapeBaseState parent, params Shape[] myShapes) : base(stateName, parent)
        {
            this.myShapes = new HashSet<Shape>(myShapes);
        }

        public bool IsMe(HashSet<Shape> shapes)
        {
            return shapes.SetEquals(myShapes);
        }
    }
}
