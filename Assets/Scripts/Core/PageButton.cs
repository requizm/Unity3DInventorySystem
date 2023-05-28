using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Clicking on this button will change the current page of the inventory.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PageButton : MonoBehaviour
    {
        [HideInInspector] public int pageIndex = -1;

        private UIInventory uiInventory;

        private void Awake()
        {
            uiInventory = ServiceLocator.Current.Get<UIInventory>();
        }

        public void SetName(string name)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = name;
        }

        public void OnClick()
        {
            if (pageIndex == -1)
            {
                Debug.LogError("Page index is not set");
                return;
            }
            uiInventory.CurrentPageIndex = pageIndex;
        }
    }
}
