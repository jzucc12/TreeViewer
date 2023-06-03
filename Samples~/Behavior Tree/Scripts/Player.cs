using UnityEngine;

namespace JZ.TreeViewer.Samples
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        void Update()
        {
            rb.MovePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
