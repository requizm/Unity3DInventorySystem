using System.Collections.Generic;
using Demo;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Holds the selected ItemSlot. <br/>
    /// Manage drag and drop of ItemSlots.
    /// </summary>
    public class UIInventoryInteractor : IGameService
    {
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
        private UIInventoryManager _uiInventoryManager;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            _uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
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

                    var fromStackLimit = from.Items[0].ItemAsset.StackLimit;
                    var toStackLimit = to.Items[0].ItemAsset.StackLimit;

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

            var pageIndex = _uiInventoryManager.CurrentPageIndex;
            var fromItemIndex = pageIndex * inventoryManager.PageLimit + fromSiblingIndex;
            var toItemIndex = pageIndex * inventoryManager.PageLimit + toSiblingIndex;

            var fromItemsCopy = new List<IItem>(from.Items);
            var toItemsCopy = new List<IItem>(to.Items);

            // Change backend order
            inventoryManager.Items[fromItemIndex] = swapNeeded ? toItemsCopy : fromItemsCopy;
            inventoryManager.Items[toItemIndex] = swapNeeded ? fromItemsCopy : toItemsCopy;

            if (SelectedSlot != null && SelectedSlot.IsEmpty)
            {
                SelectedSlot = null;
            }
        }
    }
}
