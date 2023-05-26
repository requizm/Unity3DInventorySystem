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

        private void OnInventoryToggle(bool isInventoryOpen)
        {
            if (isInventoryOpen) return;

            uiInventory.ItemSlots.ForEach(itemSlot => itemSlot.ResetColor());
            uiInventory.searchInput.text = string.Empty;
        }

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
                if (firstItem.ItemAsset.assetName.ToLower().Contains(value.ToLower()))
                {
                    foundedItemSlots.Add(itemSlot);
                    continue;
                }

                if (firstItem.ItemAsset.tags.Any(tag => tag.ToLower().Contains(value.ToLower())))
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
