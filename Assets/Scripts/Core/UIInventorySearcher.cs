using System.Collections.Generic;
using System.Linq;
using Demo;

namespace Core
{
    public class UIInventorySearcher : IGameService
    {
        private UIInventory uiInventory;
        private UIManager uiManager;

        public void Initialize()
        {
            uiInventory = ServiceLocator.Current.Get<UIInventory>();
            uiInventory.searchInput.onValueChanged.AddListener(OnValueChanged);
            uiManager = ServiceLocator.Current.Get<UIManager>();

            uiManager.OnInventoryToggle += OnInventoryToggle;
        }

        /// <summary>
        /// Reset search input and item slots colors when inventory is closed
        /// </summary>
        /// <param name="isInventoryOpen"></param>
        private void OnInventoryToggle(bool isInventoryOpen)
        {
            if (isInventoryOpen) return;

            uiInventory.ItemSlots.ForEach(itemSlot => itemSlot.ResetColor());
            uiInventory.searchInput.text = string.Empty;
        }

        /// <summary>
        /// Search items by name or tag
        /// </summary>
        /// <param name="value"></param>
        private void OnValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                uiInventory.ItemSlots.ForEach(itemSlot => itemSlot.ResetColor());
                return;
            }

            var foundedItemSlots = new List<ItemSlot>();
            foreach (var itemSlot in uiInventory.ItemSlots)
            {
                if (itemSlot.IsEmpty)
                {
                    continue;
                }

                var firstItem = itemSlot.Items[0];
                if (firstItem.ItemAsset.AssetName.ToLower().Contains(value.ToLower()))
                {
                    foundedItemSlots.Add(itemSlot);
                    continue;
                }

                if (firstItem.ItemAsset.Tags.Any(tag => tag.ToLower().Contains(value.ToLower())))
                {
                    foundedItemSlots.Add(itemSlot);
                    continue;
                }
            }

            uiInventory.ItemSlots.ForEach(itemSlot =>
            {
                if (foundedItemSlots.Contains(itemSlot))
                {
                    itemSlot.ResetColor();
                    return;
                }

                itemSlot.SetColor(itemSlot.NotFilledIconColor);
            });
        }
    }
}
