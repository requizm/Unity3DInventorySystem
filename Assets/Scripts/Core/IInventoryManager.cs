using System;
using System.Collections.Generic;

namespace Core
{
    public interface IInventoryManager
    {
        public Action<IItem> OnItemAdded { get; set; }
        public Action<IItem> OnItemRemoved { get; set; }

        public List<IItem> Items { get; set; }
        public int Count { get; }
        public int Limit { get; set; }

        public void AddItem(IItem item);

        public void RemoveItem(IItem item);
    }
}
