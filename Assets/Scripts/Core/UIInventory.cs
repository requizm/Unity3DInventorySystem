using Demo;
using UnityEngine;

namespace Core
{
    public class UIInventory : MonoBehaviour, IGameService
    {
        [SerializeField] public GameObject inventoryPanel;
        [SerializeField] public GameObject itemSlotPrefab;

        private UIInventoryManager uiInventoryManager;
        private InventoryManager inventoryManager;
        public void Initialize()
        {
            uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            
            Refresh();
            inventoryManager.OnItemAdded += OnItemAdded;
            inventoryManager.OnItemRemoved += OnItemRemoved;
        }
        
        /// <summary>
        /// Checks if the item can be dropped
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && uiInventoryManager.SelectedSlot != null && !uiInventoryManager.SelectedSlot.IsEmpty)
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
        
        /// <summary>
        /// Refresh inventory UI
        /// </summary>
        private void Refresh()
        {
            foreach (Transform child in inventoryPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in inventoryManager.Items)
            {
                var itemSlot = Instantiate(itemSlotPrefab, inventoryPanel.transform);
                var itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
                if (itemSlotComponent == null)
                {
                    Debug.LogError("ItemSlot component not found");
                    continue;
                }

                if (item.Count > 0)
                {
                    itemSlotComponent.SetItem(item);
                }
                else
                {
                    itemSlotComponent.Clear();
                }
            }
        }
        
        /// <summary>
        /// Called when item is added to inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemAdded(IItem item, int index)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();

            var itemSlot = itemSlots[index];
            itemSlot.SetItem(inventoryManager.Items[index]);
        }

        /// <summary>
        /// Called when item is removed from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemRemoved(IItem item, int index)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();

            var itemSlot = itemSlots[index];
            itemSlot.Remove(item);
        }

        private void OnDestroy()
        {
            inventoryManager.OnItemAdded -= OnItemAdded;
            inventoryManager.OnItemRemoved -= OnItemRemoved;
        }
    }
}
