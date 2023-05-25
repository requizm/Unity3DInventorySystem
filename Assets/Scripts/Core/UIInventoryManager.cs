using System;
using Demo;
using UnityEngine;

namespace Core
{
    public class UIInventoryManager : MonoBehaviour, IGameService
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject itemSlotPrefab;

        private ItemSlot selectedItem;

        public ItemSlot SelectedItem
        {
            get => selectedItem;
            private set
            {
                if (selectedItem != null)
                {
                    selectedItem.OnDeselect();
                }

                selectedItem = value;
                
                if (selectedItem != null)
                {
                    selectedItem.OnSelect();
                }
            }
        }

        private InventoryManager inventoryManager;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();

            Refresh();
            inventoryManager.OnItemAdded += OnItemAdded;
            inventoryManager.OnItemRemoved += OnItemRemoved;
        }

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

                if (item != null)
                {
                    itemSlotComponent.SetItem(item);
                }
                else
                {
                    itemSlotComponent.Clear();
                }
            }
        }

        private void OnItemAdded(IItem item)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();
            ItemSlot availableItemSlot = null;
            foreach (var itemSlot in itemSlots)
            {
                if (itemSlot.IsEmpty)
                {
                    availableItemSlot = itemSlot;
                    break;
                }
            }

            if (availableItemSlot == null)
            {
                Debug.LogError("Item added to backend but no empty slot found!!");
                return;
            }

            availableItemSlot.SetItem(item);
        }

        private void OnItemRemoved(IItem item)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();
            ItemSlot availableItemSlot = null;
            foreach (var itemSlot in itemSlots)
            {
                if (itemSlot.Item == item)
                {
                    availableItemSlot = itemSlot;
                    break;
                }
            }

            if (availableItemSlot == null)
            {
                Debug.LogError("Item removed from backend but no slot found!!");
                return;
            }

            availableItemSlot.Clear();
        }

        public void ClickItem(ItemSlot item)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();
            var previousItemSlot = Array.Find(itemSlots, x => x == SelectedItem);
            var newItemSlot = Array.Find(itemSlots, x => x == item);

            if (previousItemSlot != null && !SelectedItem.IsEmpty && previousItemSlot.Item == newItemSlot.Item)
            {
                SelectedItem = null;
            }
            else
            {
                if (selectedItem == null || !SelectedItem.IsEmpty)
                {
                    SelectedItem = newItemSlot;
                }
            }
        }

        private void OnDestroy()
        {
            inventoryManager.OnItemAdded -= OnItemAdded;
            inventoryManager.OnItemRemoved -= OnItemRemoved;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && SelectedItem != null && !SelectedItem.IsEmpty)
            {
                var item = SelectedItem.Item as IPickable;
                if (item == null)
                {
                    Debug.LogError("Item is not pickable");
                    return;
                }
                SelectedItem = null;
                item.Drop();
            }
        }
    }
}
