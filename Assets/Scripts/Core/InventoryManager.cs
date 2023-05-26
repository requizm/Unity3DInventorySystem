using System;
using System.Collections.Generic;
using System.Linq;
using Demo;
using UnityEngine;

namespace Core
{
    public class InventoryManager : IGameService
    {
        public Action<IItem> OnItemAdded { get; set; }
        public Action<IItem> OnItemRemoved { get; set; }
        public List<IItem> Items { get; set; }
        public int Count => Items.Count(i => i != null);
        public int Limit { get; set; } = 15; // TODO: Make it dynamic

        public InventoryManager()
        {
            Items = new List<IItem>();
            Items.Capacity = Limit;

            for (int i = 0; i < Items.Capacity; i++)
            {
                Items.Add(null);
            }
        }

        public void Initialize()
        {
            Debug.Log($"InventoryManager initialized");
        }

        public void AddItem(IItem item)
        {
            if (Items.Any(i => i != null && i.Id == item.Id))
            {
                Debug.LogError($"{item.ItemAsset.name}:{item.Id} already exists");
                return;
            }

            var index = Items.FindIndex(i => i == null);
            if (index == -1)
            {
                Debug.LogError($"Inventory is full");
                return;
            }

            Items[index] = item;
            item.OnAdd();
            OnItemAdded?.Invoke(item);
            Debug.Log($"{item.ItemAsset.name}:{item.Id} added");
        }

        public void RemoveItem(IItem item)
        {
            if (Items.All(i => i != null && i.Id != item.Id))
            {
                Debug.LogError($"{item.ItemAsset.name}:{item.Id} does not exist");
                return;
            }

            var index = Items.FindIndex(i => i.Id == item.Id);
            Items[index] = null;
            item.OnRemove();
            OnItemRemoved?.Invoke(item);
            Debug.Log($"{item.ItemAsset.name}:{item.Id} removed");
        }
    }
}
