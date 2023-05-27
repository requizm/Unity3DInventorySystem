using System;
using Core;
using UnityEngine;

namespace Tests
{
    public class TestItem : IItem
    {
        public IItemAsset ItemAsset { get; }
        public int Id { get; }
        public void OnAdd()
        {
            
        }

        public void OnRemove()
        {
            
        }
        
        public TestItem(int id, IItemAsset itemAsset)
        {
            Id = id;
            ItemAsset = itemAsset;
        }
    }
}
