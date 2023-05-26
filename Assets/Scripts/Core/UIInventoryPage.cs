﻿using System.Collections.Generic;
using Demo;
using UnityEngine;

namespace Core
{
    public class UIInventoryPage : MonoBehaviour
    {
        [HideInInspector] public GameObject inventoryPanel;
        [HideInInspector] public int pageIndex;

        private GameObject itemSlotPrefab;


        public List<ItemSlot> ItemSlots { get; set; } = new List<ItemSlot>();

        private InventoryManager inventoryManager;
        private UIInventory uiInventory;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            uiInventory = ServiceLocator.Current.Get<UIInventory>();

            itemSlotPrefab = uiInventory.itemSlotPrefab;

            Refresh();
            inventoryManager.OnItemAdded += OnItemAdded;
            inventoryManager.OnItemRemoved += OnItemRemoved;
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

            var items = inventoryManager.Items.GetRange(pageIndex * inventoryManager.PageLimit,
                inventoryManager.PageLimit);

            foreach (var item in items)
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

                ItemSlots.Add(itemSlotComponent);
            }
        }

        /// <summary>
        /// Called when item is added to inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemAdded(IItem item, int index)
        {
            var realIndex = index - pageIndex * inventoryManager.PageLimit;
            if (realIndex < 0 || realIndex >= ItemSlots.Count)
            {
                return;
            }

            var itemSlot = ItemSlots[realIndex];
            itemSlot.SetItem(inventoryManager.Items[realIndex]);
        }

        /// <summary>
        /// Called when item is removed from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        private void OnItemRemoved(IItem item, int index)
        {
            var realIndex = index - pageIndex * inventoryManager.PageLimit;
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
        }
    }
}
