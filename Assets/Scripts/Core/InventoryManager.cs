using System;
using System.Collections.Generic;
using System.Linq;
using Demo;
using UnityEngine;

namespace Core
{
    public class InventoryManager : IGameService
    {
        public Action<IItem, int> OnItemAdded { get; set; }
        public Action<IItem, int> OnItemRemoved { get; set; }
        public List<List<IItem>> Items { get; set; }
        public int Count => Items.Count(i => i.Count > 0);
        public int Limit { get; set; } = 15; // TODO: Make it dynamic

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
            Debug.Log($"InventoryManager initialized");
        }

        public void AddItem(IItem item)
        {
            // Check if item is stackable
            var stackIndex = Items.FindIndex(i => i.Any(j => j.ItemAsset == item.ItemAsset && i.Count < j.ItemAsset.stackLimit));
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
            Debug.Log($"{item.ItemAsset.name}:{item.Id} added");
        }

        public void RemoveItem(IItem item)
        {
            var stackIndex = Items.FindIndex(i => i.Contains(item));
            if (stackIndex == -1)
            {
                Debug.LogError($"Item {item.ItemAsset.name}:{item.Id} not found");
                return;
            }

            Items[stackIndex].Remove(item);

            item.OnRemove();
            OnItemRemoved?.Invoke(item, stackIndex);
            Debug.Log($"{item.ItemAsset.name}:{item.Id} removed");
        }
    }
}
