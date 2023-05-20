using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// HSM for showing shape combinations. Tree can only be viewed in play mode
    /// </summary>
    public class ShapeHSM : MonoBehaviour, ITreeViewer
    {
        [SerializeField] private ShapeChanger[] changers;
        [SerializeField] private Image[] outputs;
        private ShapeBaseState origin;
        private ShapeBaseState currentState;


        #region //HSM Specific
        private void Awake()
        {
            origin = new ShapeBaseState("Origin");

            var same = new ShapeBaseState("None unique", origin);
            var circle = new ShapeShowState("Circle", same, Shape.Circle);
            var triangle = new ShapeShowState("Triangle", same, Shape.Triangle);
            var square = new ShapeShowState("Square", same, Shape.Square);

            var mixed = new ShapeBaseState("Some unique", origin);
            var cone = new ShapeShowState("Cone", mixed, new Shape[] { Shape.Circle, Shape.Triangle });
            var capsule = new ShapeShowState("Capsule", mixed, new Shape[] { Shape.Circle, Shape.Square });
            var hourglass = new ShapeShowState("Hourglass", mixed, new Shape[] { Shape.Triangle, Shape.Square });

            var unique = new ShapeBaseState("All unique", origin);
            var all = new ShapeShowState("Pencil", unique, new Shape[] { Shape.Circle, Shape.Triangle, Shape.Square });
        }

        private void Start()
        {
            DetermineState();
        }

        private void OnEnable()
        {
            foreach(ShapeChanger changer in changers)
            {
                changer.OnShapeChange += DetermineState;
            }
        }

        private void OnDisable()
        {
            foreach(ShapeChanger changer in changers)
            {
                changer.OnShapeChange -= DetermineState;
            }
        }

        private void DetermineState()
        {
            HashSet<Shape> shapes = new HashSet<Shape>();
            foreach(ShapeChanger changer in changers)
            {
                shapes.Add(changer.currentShape);
            }


            foreach(ShapeShowState state in origin.Where(child => child is ShapeShowState))
            {
                if(!state.IsMe(shapes)) continue;
                if(currentState != null)
                {
                    GetCurrentImage().enabled = false;
                }
                currentState?.ExitState();
                currentState = state;
                currentState?.EnterState();
                GetCurrentImage().enabled = true;
                break;
            }
        }

        private Image GetCurrentImage()
        {
            return outputs.First(image => image.name == currentState.GetNodeName());
        }
        #endregion

        #region //Interface specific
        public string GetTreeName()
        {
            return name;
        }

        public IEnumerable<ITreeNodeViewer> GetAllNodes()
        {
            return origin;
        }
        #endregion
    }
}