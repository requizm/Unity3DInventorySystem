using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// This is basically show/hide UI on Tab press.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;

        private void Awake()
        {
            canvas.SetActive(false);
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (canvas.activeSelf)
                {
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.visible = true;
                }

                canvas.SetActive(!canvas.activeSelf);
            }
        }
    }
}
