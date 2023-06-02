using System.Collections.Generic;
using System.Linq;
using Demo;
using TMPro;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the pages of the inventory. <br/>
    /// When the game starts, it creates the pages and the page buttons.
    /// </summary>
    public class UIInventoryManager : MonoBehaviour, IGameService
    {
        // TODO: Add https://github.com/dbrizov/NaughtyAttributes to the project and use it for validation.
        [Header("Prefabs")] [SerializeField] public GameObject pagesPanel;
        [SerializeField] public GameObject pageButtonsPanel;
        [SerializeField] public GameObject itemSlotPrefab;
        [SerializeField] public TMP_InputField searchInput;
        [SerializeField] private GameObject contentPrefab;
        [SerializeField] private GameObject pageButtonPrefab;

        private List<UIInventoryPage> UIInventoryPages { get; } = new List<UIInventoryPage>();

        private UIInventoryPage currentPage;

        public UIInventoryPage CurrentPage
        {
            get => currentPage;
            private set
            {
                if (currentPage != null)
                {
                    currentPage.gameObject.SetActive(false);
                }

                currentPage = value;
                currentPage.gameObject.SetActive(true);
            }
        }

        private int currentPageIndex = -1;

        public int CurrentPageIndex
        {
            get => currentPageIndex;
            set
            {
                if (currentPageIndex == value)
                {
                    return;
                }

                currentPageIndex = value;
                CurrentPage = UIInventoryPages[currentPageIndex];
            }
        }

        public List<ItemSlot> ItemSlots
        {
            get { return UIInventoryPages.SelectMany(x => x.ItemSlots).ToList(); }
        }

        private InventoryManager inventoryManager;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();

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
                pageComponent.InventoryPanel = pageGameObject;
                pageComponent.PageIndex = i;
                pageComponent.Initialize();
                UIInventoryPages.Add(pageComponent);
            }

            CurrentPageIndex = 0;
        }
        
        public ItemSlot GetItemSlot(IItem item)
        {
            return ItemSlots.FirstOrDefault(x => x.Items.Any(y => y == item));
        }
    }
}
