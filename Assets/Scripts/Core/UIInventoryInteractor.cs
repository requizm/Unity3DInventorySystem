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
        private UIInventoryManager uiInventoryManager;

        public void Initialize()
        {
            inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
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

            var fromIndex = uiInventoryManager.ItemSlots.IndexOf(from);
            var toIndex = uiInventoryManager.ItemSlots.IndexOf(to);

            inventoryManager.SwapSlots(fromIndex, toIndex);

            if (SelectedSlot != null && SelectedSlot.IsEmpty)
            {
                SelectedSlot = null;
            }
        }
    }
}
