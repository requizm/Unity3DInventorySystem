using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Tests
{
    public class TestItemAsset : IItemAsset
    {
        public int Id { get; }
        public string AssetName { get; }
        public List<string> Tags { get; }
        public int StackLimit { get; }
        public GameObject Prefab { get; }
        public Sprite Icon { get; }
        
        public TestItemAsset(int id, string assetName, List<string> tags, int stackLimit, GameObject prefab, Sprite icon)
        {
            Id = id;
            AssetName = assetName;
            Tags = tags;
            StackLimit = stackLimit;
            Prefab = prefab;
            Icon = icon;
        }
    }
}
