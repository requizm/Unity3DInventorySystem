using System.Collections.Generic;
using System.Linq;
using Demo;
using TMPro;
using UnityEngine;

namespace Core
{
    public class UIInventory : MonoBehaviour, IGameService
    {
        [Header("Prefabs")] [SerializeField] public GameObject pagesPanel;
        [SerializeField] public GameObject pageButtonsPanel;
        [SerializeField] public GameObject itemSlotPrefab;
        [SerializeField] public TMP_InputField searchInput;
        [SerializeField] private GameObject contentPrefab;
        [SerializeField] private GameObject pageButtonPrefab;

        public List<UIInventoryPage> UIInventoryPages { get; set; } = new List<UIInventoryPage>();

        private UIInventoryPage currentPage;

        public UIInventoryPage CurrentPage
        {
            get => currentPage;
            set
            {
                if (currentPage != null)
                {
                    currentPage.gameObject.SetActive(false);
                }

                currentPage = value;
                currentPage.gameObject.SetActive(true);
            }
        }

        public List<ItemSlot> ItemSlots
        {
            get { return UIInventoryPages.SelectMany(x => x.ItemSlots).ToList(); }
        }

        private InventoryManager inventoryManager;
        private UIInventoryManager uiInventoryManager;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

            var pageCount = inventoryManager.Limit / inventoryManager.PageLimit;
            for (var i = 0; i < pageCount; i++)
            {
                var pageButtonGameObject = Instantiate(pageButtonPrefab, pageButtonsPanel.transform);
                var pageButtonComponent = pageButtonGameObject.GetComponent<PageButton>();
                pageButtonComponent.pageIndex = i;
                pageButtonComponent.SetName((i + 1).ToString());


                var pageGameObject = Instantiate(contentPrefab, pagesPanel.transform);
                pageGameObject.SetActive(false);
                var pageComponent = pageGameObject.GetComponent<UIInventoryPage>();
                pageComponent.inventoryPanel = pageGameObject;
                pageComponent.pageIndex = i;
                pageComponent.Initialize();
                UIInventoryPages.Add(pageComponent);
            }

            CurrentPage = UIInventoryPages[0];
        }

        /// <summary>
        /// Checks if the item can be dropped
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && uiInventoryManager.SelectedSlot != null &&
                !uiInventoryManager.SelectedSlot.IsEmpty)
            {
                var item = uiInventoryManager.SelectedSlot.Items[^1] as Pickable;
                if (item == null)
                {
                    Debug.LogError("Item is not pickable");
                    return;
                }

                item.Drop();
            }
        }
    }
}
