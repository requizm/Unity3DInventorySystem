﻿using System.Collections.Generic;
using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Single page of inventory. Manages slots of a page. <br/>
    /// When the page is created, it creates the empty item slots.
    /// </summary>
    public class UIInventoryPage : MonoBehaviour
    {
        public GameObject InventoryPanel { get; set; }
        public int PageIndex { get; set; } = -1;

        [HideInInspector] public GameObject itemSlotPrefab;


        public List<ItemSlot> ItemSlots { get; } = new List<ItemSlot>();

        private InventoryManager inventoryManager;

        public void Initialize()
        {
            // TODO: Add https://github.com/dbrizov/NaughtyAttributes to the project and use it for validation.
            if (InventoryPanel == null || PageIndex == -1)
            {
                Debug.LogError("Inventory panel or page index not set");
                return;
            }

            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();

            Refresh();
            inventoryManager.OnItemAdded += OnItemAdded;
            inventoryManager.OnItemRemoved += OnItemRemoved;
            inventoryManager.OnSlotsSwapped += OnSlotSwapped;
        }

        /// <summary>
        /// Refresh inventory UI.
        /// </summary>
        private void Refresh()
        {
            foreach (Transform child in InventoryPanel.transform)
            {
                Destroy(child.gameObject);
            }

            var items = inventoryManager.Items.GetRange(PageIndex * inventoryManager.PageLimit,
                inventoryManager.PageLimit);

            foreach (var item in items)
            {
                var itemSlot = Instantiate(itemSlotPrefab, InventoryPanel.transform);
                var itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
                if (itemSlotComponent == null)
                {
                    Debug.LogError("ItemSlot component not found");
                    continue;
                }

                itemSlotComponent.Initialize();

                if (item.Count > 0)
                {
                    itemSlotComponent.SetItem(item);
                }
                else
                {
                    itemSlotComponent.Clear();
                }

                ItemSlots.Add(itemSlotComponent);
            }
        }

        /// <summary>
        /// Called when item is added to inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemAdded(IItem item, int index)
        {
            var realIndex = index - PageIndex * inventoryManager.PageLimit;
            if (realIndex < 0 || realIndex >= ItemSlots.Count)
            {
                return;
            }

            var itemSlot = ItemSlots[realIndex];
            itemSlot.SetItem(inventoryManager.Items[index]);
        }

        /// <summary>
        /// Called when item is removed from inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemRemoved(IItem item, int index)
        {
            var realIndex = index - PageIndex * inventoryManager.PageLimit;
            if (realIndex < 0 || realIndex >= ItemSlots.Count)
            {
                return;
            }

            var itemSlot = ItemSlots[realIndex];
            itemSlot.Remove(item);
        }

        private void OnDestroy()
        {
            inventoryManager.OnItemAdded -= OnItemAdded;
            inventoryManager.OnItemRemoved -= OnItemRemoved;
            inventoryManager.OnSlotsSwapped -= OnSlotSwapped;
        }
        
        /// <summary>
        /// Called when items are swapped.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void OnSlotSwapped(int from, int to)
        {
            var realFrom = from - PageIndex * inventoryManager.PageLimit;
            var realTo = to - PageIndex * inventoryManager.PageLimit;
            if (realFrom < 0 || realFrom >= ItemSlots.Count || realTo < 0 || realTo >= ItemSlots.Count)
            {
                return;
            }

            ItemSlots[realFrom].SetItem(new List<IItem>(inventoryManager.Items[from]));
            ItemSlots[realTo].SetItem(new List<IItem>(inventoryManager.Items[to]));
        }
    }
}
