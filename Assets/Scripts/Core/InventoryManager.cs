using System;
using System.Collections.Generic;
using System.Linq;
using Demo;
using UnityEngine;

namespace Core
{
    public class InventoryManager : IGameService
    {
        /// <summary>
        /// Called when item is added to inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        public Action<IItem, int> OnItemAdded { get; set; }

        /// <summary>
        /// Called when item is removed from inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">Index of the stack</param>
        public Action<IItem, int> OnItemRemoved { get; set; }

        /// <summary>
        /// Current items in inventory.
        /// </summary>
        public List<List<IItem>> Items { get; set; }

        /// <summary>
        /// Count of items in inventory.
        /// </summary>
        public int Count => Items.Count(i => i.Count > 0);

        /// <summary>
        /// Limit of items in per page. Default is 15.
        /// </summary>
        public int PageLimit { get; set; } = 15; // TODO: Make it dynamic

        /// <summary>
        /// Limit of items in inventory. Default is Limit * 2.
        /// </summary>
        public int Limit => PageLimit * 2;

        public InventoryManager()
        {
            Items = new List<List<IItem>>();
            Items.Capacity = Limit;

            for (int i = 0; i < Items.Capacity; i++)
            {
                Items.Add(new List<IItem>());
            }
        }

        public void Initialize()
        {
        }

        /// <summary>
        /// Add item to inventory.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IItem item)
        {
            // Check if item is stackable
            var stackIndex = Items.FindIndex(i =>
                i.Any(j => j.ItemAsset == item.ItemAsset && i.Count > 0 && i.Count < j.ItemAsset.StackLimit));
            if (stackIndex == -1)
            {
                stackIndex = Items.FindIndex(i => i.Count == 0);
                if (stackIndex == -1)
                {
                    Debug.LogError($"Inventory is full");
                    return;
                }
            }

            Items[stackIndex].Add(item);

            item.OnAdd();
            OnItemAdded?.Invoke(item, stackIndex);
            Debug.Log($"{item.ItemAsset.AssetName}:{item.Id} added");
        }

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(IItem item)
        {
            var stackIndex = Items.FindIndex(i => i.Contains(item));
            if (stackIndex == -1)
            {
                Debug.LogError($"Item {item.ItemAsset.AssetName}:{item.Id} not found");
                return;
            }

            item.OnRemove();
            OnItemRemoved?.Invoke(item, stackIndex);
            Items[stackIndex].Remove(item);

            Debug.Log($"{item.ItemAsset.AssetName}:{item.Id} removed");
        }
        
        public Tuple<int, int> GetItemIndex(IItem item)
        {
            var stackIndex = Items.FindIndex(i => i.Contains(item));
            if (stackIndex == -1)
            {
                Debug.LogError($"Item {item.ItemAsset.AssetName}:{item.Id} not found");
                return null;
            }

            var itemIndex = Items[stackIndex].FindIndex(i => i == item);
            return new Tuple<int, int>(stackIndex, itemIndex);
        }
        
        public int GetEmptySlot()
        {
            return Items.FindIndex(i => i.Count == 0);
        }
    }
}
