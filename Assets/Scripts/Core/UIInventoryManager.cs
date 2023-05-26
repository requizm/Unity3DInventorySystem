﻿using System;
using Demo;
using UnityEngine;

namespace Core
{
    public class UIInventoryManager : MonoBehaviour, IGameService
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject itemSlotPrefab;

        private ItemSlot selectedSlot;

        public ItemSlot SelectedSlot
        {
            get => selectedSlot;
            set
            {
                if (selectedSlot != null)
                {
                    selectedSlot.OnDeselect();
                }

                selectedSlot = value;

                if (selectedSlot != null)
                {
                    selectedSlot.OnSelect();
                }
            }
        }

        private ItemSlot dragStartItemSlot, dragEndItemSlot;

        public ItemSlot DragStartItemSlot
        {
            get { return dragStartItemSlot; }
            set
            {
                if (dragStartItemSlot != null)
                {
                    dragStartItemSlot.OnDragEnd(false);
                }

                dragStartItemSlot = value;

                if (dragStartItemSlot != null)
                {
                    dragStartItemSlot.OnDragStart();
                }

                dragEndItemSlot = null;
            }
        }

        public ItemSlot DragEndItemSlot
        {
            get { return dragEndItemSlot; }
            set
            {
                if (dragEndItemSlot != null)
                {
                    dragEndItemSlot.OnDragEnd(false);
                }

                dragEndItemSlot = value;

                if (dragEndItemSlot != null)
                {
                    dragEndItemSlot.OnDragEnd(true);
                }

                dragStartItemSlot = null;
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

        private void OnItemAdded(IItem item, int index)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();

            var itemSlot = itemSlots[index];
            itemSlot.SetItem(inventoryManager.Items[index]);
        }

        private void OnItemRemoved(IItem item, int index)
        {
            var itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();

            var itemSlot = itemSlots[index];
            itemSlot.Decrease(item);
        }

        private void OnDestroy()
        {
            inventoryManager.OnItemAdded -= OnItemAdded;
            inventoryManager.OnItemRemoved -= OnItemRemoved;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && SelectedSlot != null && !SelectedSlot.IsEmpty)
            {
                var item = SelectedSlot.Item[^1] as Pickable;
                if (item == null)
                {
                    Debug.LogError("Item is not pickable");
                    return;
                }

                item.Drop();
            }
        }

        public void SwapTwoItems(ItemSlot itemSlot1, ItemSlot itemSlot2)
        {
            if (itemSlot1 == null || itemSlot2 == null)
            {
                Debug.LogError("ItemSlot is null");
                return;
            }

            if (itemSlot1 == itemSlot2)
            {
                Debug.LogError("Both ItemSlot is same");
                return;
            }

            if (itemSlot1.IsEmpty)
            {
                Debug.LogError("ItemSlot1 is empty");
                return;
            }

            var itemSlot1Transform = itemSlot1.transform;
            var itemSlot2Transform = itemSlot2.transform;
            var itemSlot1SiblingIndex = itemSlot1Transform.GetSiblingIndex();
            var itemSlot2SiblingIndex = itemSlot2Transform.GetSiblingIndex();
            itemSlot1Transform.SetSiblingIndex(itemSlot2SiblingIndex);
            itemSlot2Transform.SetSiblingIndex(itemSlot1SiblingIndex);

            // Change backend order
            inventoryManager.Items[itemSlot1SiblingIndex] = itemSlot2.Item;
            inventoryManager.Items[itemSlot2SiblingIndex] = itemSlot1.Item;
        }
    }
}
