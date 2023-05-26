using System.Collections.Generic;
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

        /// <summary>
        /// Checks if the item can be dropped
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && SelectedSlot != null && !SelectedSlot.IsEmpty)
            {
                var item = SelectedSlot.Items[^1] as Pickable;
                if (item == null)
                {
                    Debug.LogError("Item is not pickable");
                    return;
                }

                item.Drop();
            }
        }

        /// <summary>
        /// Swaps two items. If the items are of same type, they will be merged.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SwapTwoItems(ItemSlot from, ItemSlot to)
        {
            if (from == null || to == null)
            {
                Debug.LogError("ItemSlot is null");
                return;
            }

            if (from == to)
            {
                Debug.LogError("Both ItemSlot is same");
                return;
            }

            if (from.IsEmpty)
            {
                Debug.LogError("ItemSlot1 is empty");
                return;
            }

            var swapNeeded = true;
            if (!to.IsEmpty)
            {
                if (from.Items.GetType() == to.Items.GetType())
                {
                    var fromList = from.Items;
                    var toList = to.Items;

                    var fromCount = fromList.Count;
                    var toCount = toList.Count;

                    var fromStackLimit = from.Items[0].ItemAsset.stackLimit;
                    var toStackLimit = to.Items[0].ItemAsset.stackLimit;

                    if (fromCount + toCount <= fromStackLimit)
                    {
                        var items = new List<IItem>();
                        items.AddRange(fromList);
                        items.AddRange(toList);
                        to.SetItem(items);
                        from.Clear();
                        swapNeeded = false;
                    }
                    else if (toCount < toStackLimit)
                    {
                        var items = new List<IItem>();
                        items.AddRange(toList);
                        var count = toStackLimit - toCount;
                        items.AddRange(fromList.GetRange(fromCount - count, count));
                        to.SetItem(items);
                        from.Decrease(count);
                        swapNeeded = false;
                    }
                }
            }

            var fromTransform = from.transform;
            var toTransform = to.transform;
            var fromSiblingIndex = fromTransform.GetSiblingIndex();
            var toSiblingIndex = toTransform.GetSiblingIndex();
            if (swapNeeded)
            {
                fromTransform.SetSiblingIndex(toSiblingIndex);
                toTransform.SetSiblingIndex(fromSiblingIndex);
            }

            // Change backend order
            inventoryManager.Items[fromSiblingIndex] = swapNeeded ? to.Items : from.Items;
            inventoryManager.Items[toSiblingIndex] = swapNeeded ? from.Items : to.Items;

            if (SelectedSlot != null && SelectedSlot.IsEmpty)
            {
                SelectedSlot = null;
            }
        }
    }
}
