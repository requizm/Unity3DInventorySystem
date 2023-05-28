using System;
using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Basically show/hide UI on Tab press.
    /// </summary>
    public class UIManager : MonoBehaviour, IGameService
    {
        [SerializeField] private GameObject canvas;

        public Action<bool> OnInventoryToggle;

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
                OnInventoryToggle?.Invoke(canvas.activeSelf);
            }
        }

        public void Initialize()
        {
        }
    }
}
