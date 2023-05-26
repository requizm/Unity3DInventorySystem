using Demo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    [RequireComponent(typeof(Button))]
    public class PageButton : MonoBehaviour
    {
        public int pageIndex;

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
            uiInventory.CurrentPage = uiInventory.UIInventoryPages[pageIndex];
        }
    }
}
