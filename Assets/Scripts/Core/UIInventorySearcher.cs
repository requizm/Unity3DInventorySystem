using System.Collections.Generic;
using System.Linq;
using Demo;

namespace Core
{
    public class UIInventorySearcher : IGameService
    {
        private UIInventoryManager uiInventoryManager;
        private UIManager uiManager;

        public void Initialize()
        {
            uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            uiInventoryManager.searchInput.onValueChanged.AddListener(OnValueChanged);
            uiManager = ServiceLocator.Current.Get<UIManager>();

            uiManager.OnInventoryToggle += OnInventoryToggle;
        }

        /// <summary>
        /// Reset search input and item slots colors when inventory is closed.
        /// </summary>
        /// <param name="isInventoryOpen"></param>
        private void OnInventoryToggle(bool isInventoryOpen)
        {
            if (isInventoryOpen) return;

            uiInventoryManager.ItemSlots.ForEach(itemSlot => itemSlot.ResetColor());
            uiInventoryManager.searchInput.text = string.Empty;
        }

        /// <summary>
        /// Search items by name or tag.
        /// </summary>
        /// <param name="value"></param>
        private void OnValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                uiInventoryManager.ItemSlots.ForEach(itemSlot => itemSlot.ResetColor());
                return;
            }

            var foundedItemSlots = new List<ItemSlot>();
            foreach (var itemSlot in uiInventoryManager.ItemSlots)
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

            uiInventoryManager.ItemSlots.ForEach(itemSlot =>
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
