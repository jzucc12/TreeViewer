using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Player just follows the mouse cursor
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        void Update()
        {
            rb.MovePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
