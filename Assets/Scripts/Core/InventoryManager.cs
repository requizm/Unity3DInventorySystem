﻿using System;
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
        /// Called when items are swapped.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Action<int, int> OnSlotsSwapped { get; set; }

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

        /// <summary>
        /// Swap items in inventory. If items are stackable, they will be merged.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void SwapSlots(int from, int to)
        {
            if (from < 0 || from >= Items.Count || to < 0 || to >= Items.Count)
            {
                Debug.LogError($"Invalid index");
                return;
            }

            var fromStack = Items[from];
            var toStack = Items[to];

            if (fromStack.Count == 0)
            {
                Debug.LogError($"Stack {from} is empty");
                return;
            }

            bool done = false;
            if (toStack.Count > 0)
            {
                var fromItemType = fromStack[0].ItemAsset.Id;
                var toItemType = toStack[0].ItemAsset.Id;

                if (fromItemType == toItemType)
                {
                    var stackLimit = fromStack[0].ItemAsset.StackLimit;

                    var fromCount = fromStack.Count;
                    var toCount = toStack.Count;

                    if (fromCount + toCount <= stackLimit)
                    {
                        var toItems = new List<IItem>();
                        toItems.AddRange(fromStack);
                        toItems.AddRange(toStack);

                        Items[to] = toItems;
                        Items[from] = new List<IItem>();
                        done = true;
                    }
                    else if (toCount < stackLimit)
                    {
                        var toItems = new List<IItem>();
                        toItems.AddRange(toStack);
                        toItems.AddRange(fromStack.GetRange(fromCount - (stackLimit - toCount), stackLimit - toCount));

                        Items[to] = toItems;
                        Items[from] = fromStack.GetRange(0, fromCount - (stackLimit - toCount));
                        done = true;
                    }
                }
            }
            
            if (!done)
            {
                Items[from] = toStack;
                Items[to] = fromStack;
            }


            OnSlotsSwapped?.Invoke(from, to);
        }
    }
}
