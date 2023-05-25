using Demo;
using UnityEngine;

namespace Core
{
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
